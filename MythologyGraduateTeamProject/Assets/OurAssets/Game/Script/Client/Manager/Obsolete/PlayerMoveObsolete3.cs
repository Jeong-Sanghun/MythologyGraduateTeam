using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Obsolete
{

    public class PlayerMoveObsolete3 : MonoBehaviour
    {

        enum Direction
        {
            Down, Left, Right, Up
        }
        [SerializeField]
        private Camera cam;
        [SerializeField]
        private GameObject _playerObject;
        private Rigidbody2D _playerRigid;
        private ManagerGroup _dataCenter;
        private Vector2 _playerLogicalPos;
        private Dictionary<Vector2Int, MyTile> _tileDictionary;
        private Vector2 _moveDirection;
        private Vector2 _originMoveDirectionNormalized;


        private bool[] _keyMoveArray;
        private MyTile _nowStandTile;
        private float[] _speedArray;
        private int _speedIndex;
        private const float _playerColliderRadius = 0.2f;
        private float _speedTimer = 0;
        private float _mouseRightClickMaintainTime = 0;
        private bool _keyboardMoving;
        private bool _mouseMoving;
        private bool _pathFinding;
        private bool _pathFindingToggle;
        private List<Vector2> _gizmoOpenList;
        private List<Vector2> _gizmoClosedList;
        private Vector2 _gizmoMouse;
        private Vector2 _gizmoTile;
        private int PATH_FINDING_RANGE;

        // Start is called before the first frame update
        void Start()
        {
            _gizmoOpenList = new List<Vector2>();
            _gizmoClosedList = new List<Vector2>();
            _dataCenter = ManagerGroup.singleton;
            _playerLogicalPos = Vector2.zero;
            _playerRigid = _playerObject.GetComponent<Rigidbody2D>();
            //_tile_dictionary = _data_center.tile_generator.tile_dictionary;
            _tileDictionary = _dataCenter.tilemapGenerator.tileDictionary;
            _speedArray = new float[] { 0.5f, 1f, 2f, 1.5f, 1f, 0.5f };
            _speedIndex = 0;
            _keyboardMoving = false;
            _mouseMoving = false;
            _pathFinding = false;
            _pathFindingToggle = true;
            _keyMoveArray = new bool[4];
            for (int i = 0; i < _keyMoveArray.Length; i++)
            {
                _keyMoveArray[i] = false;
            }
            _nowStandTile = GetTileWithActualPos(Vector2.zero);
            PATH_FINDING_RANGE = Constants.PATH_FINDING_RANGE;

        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                SetKeyboardMove(Direction.Down, true);
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                SetKeyboardMove(Direction.Left, true);
            }
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                SetKeyboardMove(Direction.Right, true);
            }
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                SetKeyboardMove(Direction.Up, true);
            }

            if (Input.GetKeyUp(KeyCode.DownArrow))
            {
                SetKeyboardMove(Direction.Down, false);
            }
            if (Input.GetKeyUp(KeyCode.LeftArrow))
            {
                SetKeyboardMove(Direction.Left, false);
            }
            if (Input.GetKeyUp(KeyCode.RightArrow))
            {
                SetKeyboardMove(Direction.Right, false);
            }
            if (Input.GetKeyUp(KeyCode.UpArrow))
            {
                SetKeyboardMove(Direction.Up, false);
            }

            if (Input.GetMouseButtonDown(1))
            {

                OnMouseRightClickDown(Input.mousePosition);
            }
            //if (Input.GetMouseButton(1))
            //{
            //    OnMouseRightClickMaintain(Input.mousePosition);
            //}
            //if (Input.GetMouseButtonUp(1))
            //{
            //    OnMouseRightClickUp(Input.mousePosition);
            //}
            Move();

        }

        private List<MyTile> GetCollidingTilesInRadius(Vector2 _pos)
        {
            List<MyTile> _tileList = new List<MyTile>();
            bool _toggle = true;
            for (float i = -_playerColliderRadius; i <= _playerColliderRadius; i += _playerColliderRadius / 4f)
            {
                for (int j = 0; j < 2; j++)
                {
                    float _x = i;
                    float _y;
                    if (_playerColliderRadius * _playerColliderRadius - i * i <= 0)
                    {
                        _y = 0;
                    }
                    else
                    {
                        if (_toggle)
                        {
                            _y = Mathf.Sqrt(_playerColliderRadius * _playerColliderRadius - i * i);
                        }
                        else
                        {
                            _y = -Mathf.Sqrt(_playerColliderRadius * _playerColliderRadius - i * i);
                        }
                    }

                    _toggle = !_toggle;
                    Vector2 _actualPos = new Vector2(_x, _y);
                    _actualPos += _pos;
                    MyTile _tile = _tileDictionary[MyTile.ActualToLogicalPosition(_actualPos)];
                    if (!_tileList.Contains(_tile))
                    {
                        _tileList.Add(_tile);
                    }
                }

            }
            return _tileList;

        }


        private MyTile GetTileWithActualPos(Vector2 _pos)
        {
            return _tileDictionary[MyTile.ActualToLogicalPosition(_pos)];
        }

        void SetKeyboardMove(Direction _direction, bool _pressing)
        {
            _keyboardMoving = false;
            _keyMoveArray[(int)_direction] = _pressing;
            _moveDirection = Vector2.zero;
            for (int i = 0; i < _keyMoveArray.Length; i++)
            {
                if (_keyMoveArray[i] == true)
                {
                    _keyboardMoving = true;
                    switch ((Direction)i)
                    {
                        case Direction.Down:
                            _moveDirection += Vector2.down;
                            break;
                        case Direction.Left:
                            _moveDirection += Vector2.left;
                            break;
                        case Direction.Right:
                            _moveDirection += Vector2.right;
                            break;
                        case Direction.Up:
                            _moveDirection += Vector2.up;
                            break;

                    }

                }
            }
            if (_mouseMoving == true)
            {
                _mouseMoving = false;
                _pathFinding = false;
                _pathFindingToggle = !_pathFindingToggle;
            }
            if (_moveDirection == Vector2.zero)
            {
                _playerRigid.velocity = Vector2.zero;
            }
            _originMoveDirectionNormalized = _moveDirection;
            _originMoveDirectionNormalized.Normalize();

        }

        //void RightClickMove(Vector2 _mouse_canvas_pos)
        //{
        //    Vector2 _mouse_world_pos = cam.ScreenToWorldPoint(_mouse_canvas_pos);
        //    _move_direction_normalized = _mouse_world_pos-(Vector2)_player_object.transform.position;
        //    _move_direction_normalized.Normalize();
        //}
        private void OnMouseRightClickDown(Vector2 _mouse_canvas_pos)
        {
            if (_keyboardMoving == true)
            {
                _keyboardMoving = false;
                for (int i = 0; i < _keyMoveArray.Length; i++)
                {
                    _keyMoveArray[i] = false;
                }
            }

            _mouseMoving = true;
            Vector2 _mouseWorldPos = cam.ScreenToWorldPoint(_mouse_canvas_pos);
            _gizmoMouse = _mouseWorldPos;
            _mouseRightClickMaintainTime = 0;
            Vector2 _endPos = GetTileWithActualPos(_mouseWorldPos).actualPos;

            PathNode _path = GetShortestPathJPS(_playerObject.transform.position, _endPos);
            _originMoveDirectionNormalized = Vector2.zero;
            _pathFindingToggle = !_pathFindingToggle;
            if (_path != null)
            {
                _pathFinding = true;
                StartCoroutine(MoveToPath(_path));
            }
            else
            {

                _pathFinding = false;
                Vector2 _direction = _endPos - (Vector2)_playerObject.transform.position;
                _direction.Normalize();
                _originMoveDirectionNormalized = _direction;
            }
        }

        //private void OnMouseRightClickMaintain(Vector2 _mouse_canvas_pos)
        //{
        //    if (_keyboardMoving == true)
        //    {
        //        return;
        //    }
        //    Vector2 _mouseWorldPos = cam.ScreenToWorldPoint(_mouse_canvas_pos);
        //    _mouseRightClickMaintainTime += Time.deltaTime;
        //    _originMoveDirectionNormalized = _mouseWorldPos - (Vector2)_playerObject.transform.position;
        //    _originMoveDirectionNormalized.Normalize();
        //}

        //private void OnMouseRightClickUp(Vector2 _mouse_canvas_pos)
        //{
        //    if (_keyboardMoving == true)
        //    {
        //        return;
        //    }
        //    _mouseMoving = false;
        //    if (_mouseRightClickMaintainTime > 0.3f)
        //    {
        //        _originMoveDirectionNormalized = Vector2.zero;

        //        return;
        //    }
        //    Vector2 _mouse_world_pos = cam.ScreenToWorldPoint(_mouse_canvas_pos);
        //    _mouse_click_move_destination_pos = _mouse_world_pos;

        //}

        void Move()
        {
            if (_pathFinding == true)
            {
                return;
            }

            _speedTimer += Time.deltaTime;
            if (_speedTimer > 0.2)
            {
                _speedTimer = 0;
                _speedIndex++;
            }
            _playerRigid.velocity = _originMoveDirectionNormalized * _speedArray[_speedIndex % _speedArray.Length];
            _nowStandTile = GetTileWithActualPos(_playerObject.transform.position);
            List<MyTile> _tile_List = GetCollidingTilesInRadius((Vector2)_playerObject.transform.position);
            for (int i = 0; i < _tile_List.Count; i++)
            {
                _dataCenter.tilemapGenerator.SetColliderObject(_tile_List[i]);
            }
        }

        PathNode GetShortestPathAStar(Vector2 _start, Vector2 _end)
        {
            MyTile _startTile = GetTileWithActualPos(_start);

            MyTile _endTile = GetTileWithActualPos(_end);
            _gizmoTile = _endTile.actualPos;
            if (_endTile.colliding == true)
            {
                return null;
            }
            PathNode _returnPath = new PathNode(_start, _startTile, null, (_endTile.actualPos - _start).magnitude, 0);
            List<PathNode> _openPathList = new List<PathNode>();
            List<PathNode> _closePathList = new List<PathNode>();
            PathNode _nowPath = _returnPath;
            //_closePathList.Add(_nowPath);
            _gizmoClosedList.Clear();
            _gizmoOpenList.Clear();
            Vector2 _nowStartPos = _start;
            float _gValue = 0;
            while (true)
            {
                _nowStartPos = _nowPath.actualPos;
                MyTile _collidedTile = GetFirstCollidedTile(_nowStartPos, _end);
                //if (_collidedTile == null)
                if (false)
                {
                    PathNode _openPath = new PathNode(_endTile.actualPos, _endTile, _nowPath,
                        0, _nowPath.gValue + (_endTile.actualPos - _nowStartPos).magnitude);
                    _openPathList.Add(_openPath);
                    //_gizmoOpenList.Add(_openPath.actualPos);
                }
                else
                {
                    _gValue += (new Vector2(MyTile.TILE_X_SIZE, MyTile.TILE_Y_SIZE)).magnitude;
                    for (int i = 0; i < 4; i++)
                    {
                        Vector2Int _logicalPos = _nowPath.tile.logicalPos;

                        switch (i)
                        {
                            case 0:
                                _logicalPos += new Vector2Int(-1, 0);
                                break;
                            case 1:
                                _logicalPos += new Vector2Int(1, 0);
                                break;
                            case 2:
                                _logicalPos += new Vector2Int(0, 1);
                                break;
                            case 3:
                                _logicalPos += new Vector2Int(0, -1);
                                break;
                        }

                        MyTile _nowTile = _tileDictionary[_logicalPos];
                        if (_nowTile.colliding == true)
                        {
                            continue;
                        }
                        if (Mathf.Abs(_nowTile.logicalPos.x - _nowStandTile.logicalPos.x) >= PATH_FINDING_RANGE
                            || Mathf.Abs(_nowTile.logicalPos.y - _nowStandTile.logicalPos.y) >= PATH_FINDING_RANGE)
                        {
                            continue;
                        }

                        _nowStartPos = _nowTile.actualPos;
                        PathNode _openPath = null;
                        bool _inClose = false;
                        for (int j = 0; j < _openPathList.Count; j++)
                        {
                            if (_openPathList[j].tile == _nowTile)
                            {
                                _openPath = _openPathList[j];
                                break;
                            }
                        }
                        for (int j = 0; j < _closePathList.Count; j++)
                        {
                            if (_closePathList[j].tile == _nowTile)
                            {
                                _inClose = true;
                                break;
                            }
                        }
                        if (_inClose)
                        {
                            continue;
                        }

                        if (_openPath == null)
                        {
                            _openPath = new PathNode(_nowTile.actualPos, _nowTile, _nowPath,
                                (_endTile.actualPos - _nowTile.actualPos).magnitude, _gValue);
                            _openPathList.Add(_openPath);
                            //_gizmoOpenList.Add(_openPath.actualPos);
                        }
                        else
                        {
                            _openPath.UpdateValues((_endTile.actualPos - _nowTile.actualPos).magnitude, _gValue);
                        }


                    }
                }

                float _minFValue = float.MaxValue;
                int _minIndex = -1;
                for (int i = 0; i < _openPathList.Count; i++)
                {
                    PathNode _child = _openPathList[i];
                    if (_child.fValue < _minFValue)
                    {
                        _minFValue = _child.fValue;
                        _minIndex = i;
                    }
                }
                if (_openPathList.Count == 0)
                {
                    return null;
                }
                _nowPath = _openPathList[_minIndex];
                _openPathList.RemoveAt(_minIndex);
                _closePathList.Add(_nowPath);
                //_gizmoClosedList.Add(_nowPath.actualPos);
                if (_nowPath.tile == _endTile)
                {
                    return _nowPath;
                }

            }



        }


        PathNode GetShortestPathJPS(Vector2 _start, Vector2 _end)
        {
            MyTile _startTile = GetTileWithActualPos(_start);

            MyTile _endTile = GetTileWithActualPos(_end);
            _gizmoTile = _endTile.actualPos;
            if (_endTile.colliding == true)
            {
                return null;
            }
            PathNode _returnPath = new PathNode(_start, _startTile, null, (_endTile.actualPos - _start).magnitude, 0);
            List<PathNode> _openPathList = new List<PathNode>();
            List<PathNode> _closePathList = new List<PathNode>();
            PathNode _nowPath = _returnPath;
            //_closePathList.Add(_nowPath);
            _gizmoClosedList.Clear();
            _gizmoOpenList.Clear();
            Vector2 _nowStartPos = _start;
            float _gValue = 0;
            MyTile _collidedTile = GetFirstCollidedTile(_nowStartPos, _end);
            while (true)
            {
                _nowStartPos = _nowPath.actualPos;

                //if (_collidedTile == null)
                if (false)
                {
                    PathNode _openPath = new PathNode(_endTile.actualPos, _endTile, _nowPath,
                        0, _nowPath.gValue + (_endTile.actualPos - _nowStartPos).magnitude);
                    _openPathList.Add(_openPath);
                    //_gizmoOpenList.Add(_openPath.actualPos);
                }
                else
                {
                    _gValue += (new Vector2(MyTile.TILE_X_SIZE, MyTile.TILE_Y_SIZE)).magnitude;
                    for (int i = 0; i < 4; i++)
                    {
                        Vector2Int _logicalPos = _nowPath.tile.logicalPos;

                        switch (i)
                        {
                            case 0:
                                _logicalPos += new Vector2Int(-1, 0);
                                break;
                            case 1:
                                _logicalPos += new Vector2Int(1, 0);
                                break;
                            case 2:
                                _logicalPos += new Vector2Int(0, 1);
                                break;
                            case 3:
                                _logicalPos += new Vector2Int(0, -1);
                                break;
                        }

                        MyTile _nowTile = _tileDictionary[_logicalPos];
                        if (_nowTile.colliding == true)
                        {
                            continue;
                        }
                        if (Mathf.Abs(_nowTile.logicalPos.x - _nowStandTile.logicalPos.x) >= PATH_FINDING_RANGE
                            || Mathf.Abs(_nowTile.logicalPos.y - _nowStandTile.logicalPos.y) >= PATH_FINDING_RANGE)
                        {
                            continue;
                        }

                        _nowStartPos = _nowTile.actualPos;
                        PathNode _openPath = null;
                        bool _inClose = false;
                        for (int j = 0; j < _openPathList.Count; j++)
                        {
                            if (_openPathList[j].tile == _nowTile)
                            {
                                _openPath = _openPathList[j];
                                break;
                            }
                        }
                        for (int j = 0; j < _closePathList.Count; j++)
                        {
                            if (_closePathList[j].tile == _nowTile)
                            {
                                _inClose = true;
                                break;
                            }
                        }
                        if (_inClose)
                        {
                            continue;
                        }

                        if (_openPath == null)
                        {
                            _openPath = new PathNode(_nowTile.actualPos, _nowTile, _nowPath,
                                (_endTile.actualPos - _nowTile.actualPos).magnitude, _gValue);
                            _openPathList.Add(_openPath);
                            //_gizmoOpenList.Add(_openPath.actualPos);
                        }
                        else
                        {
                            _openPath.UpdateValues((_endTile.actualPos - _nowTile.actualPos).magnitude, _gValue);
                        }


                    }
                }

                float _minFValue = float.MaxValue;
                int _minIndex = -1;
                for (int i = 0; i < _openPathList.Count; i++)
                {
                    PathNode _child = _openPathList[i];
                    if (_child.fValue < _minFValue)
                    {
                        _minFValue = _child.fValue;
                        _minIndex = i;
                    }
                }
                if (_openPathList.Count == 0)
                {
                    return null;
                }
                _nowPath = _openPathList[_minIndex];
                _openPathList.RemoveAt(_minIndex);
                _closePathList.Add(_nowPath);
                //_gizmoClosedList.Add(_nowPath.actualPos);
                if (_nowPath.tile == _endTile)
                {
                    return _nowPath;
                }

            }



        }

        List<MyTile> GetTilesBetweenPosition(Vector2 _start, Vector2 _end, ref Vector2 _last_collided_pos)
        {
            List<MyTile> _tile_list = new List<MyTile>();
            MyTile _dest_tile = GetTileWithActualPos(_end);
            _end = _dest_tile.actualPos;
            Vector2 _unit_vector = _end - _start;
            _unit_vector.Normalize();
            _unit_vector *= MyTile.TILE_Y_SIZE / 5f;
            for (Vector2 _i_vec = _start; (_end - _i_vec).magnitude <= _unit_vector.magnitude; _i_vec += _unit_vector)
            {
                MyTile _tile = GetTileWithActualPos(_i_vec);
                if (!_tile_list.Contains(_tile))
                {
                    if (_tile.colliding == true && _last_collided_pos != Vector2.zero)
                    {
                        _last_collided_pos = _i_vec - _unit_vector;
                    }
                    _tile_list.Add(_tile);
                }

            }
            return _tile_list;
        }

        Vector2 GetEndCollidePosition(Vector2 _start, Vector2 _end)
        {
            List<MyTile> _tile_list = new List<MyTile>();
            Vector2 _first_collided_pos = Vector2.zero;
            MyTile _start_tile = GetTileWithActualPos(_start);
            MyTile _dest_tile = GetTileWithActualPos(_end);
            _start = _start_tile.actualPos;
            _end = _dest_tile.actualPos;
            Vector2 _unit_vector = _end - _start;
            _unit_vector.Normalize();
            _unit_vector *= MyTile.TILE_Y_SIZE / 5f;
            for (Vector2 _i_vec = _start; (_end - _i_vec).magnitude <= _unit_vector.magnitude; _i_vec += _unit_vector)
            {
                MyTile _tile = GetTileWithActualPos(_i_vec);
                if (!_tile_list.Contains(_tile))
                {
                    if (_tile.colliding == false)
                    {
                        _first_collided_pos = _i_vec;
                        return _i_vec;
                    }
                    _tile_list.Add(_tile);
                }

            }
            return _first_collided_pos;
        }
        Vector2 GetFirstCollidePosition(Vector2 _start, Vector2 _end)
        {
            List<MyTile> _tileList = new List<MyTile>();
            Vector2 _firstCollidePos = Vector2.zero;
            MyTile _destTile = GetTileWithActualPos(_end);
            _end = _destTile.actualPos;
            Vector2 _unitVector = _end - _start;
            _unitVector.Normalize();
            _unitVector *= MyTile.TILE_Y_SIZE / 8f;
            for (Vector2 _i_vec = _start; (_end - _i_vec).magnitude <= _unitVector.magnitude; _i_vec += _unitVector)
            {
                MyTile _tile = GetTileWithActualPos(_i_vec);
                if (!_tileList.Contains(_tile))
                {
                    if (_tile.colliding == true)
                    {
                        _firstCollidePos = _i_vec;
                        return _i_vec;
                    }
                    _tileList.Add(_tile);
                }

            }
            return _firstCollidePos;
        }

        MyTile GetFirstCollidedTile(Vector2 _start, Vector2 _dest)
        {
            Vector2 _firstCollidePos = Vector2.zero;
            MyTile _destTile = GetTileWithActualPos(_dest);
            MyTile _startTile = GetTileWithActualPos(_start);
            Vector2Int _destLogicalPos = _destTile.logicalPos;
            Vector2Int _startLogicalPos = _startTile.logicalPos;
            Vector2 _lineVector = _dest - _start;
            Vector2 _lineNormalVector;
            if (_lineVector == Vector2.zero)
            {
                return null;
            }
            _lineVector.Normalize();
            _lineNormalVector = new Vector2(-_lineVector.y, _lineVector.x);
            _lineNormalVector *= 0.3f;

            _lineVector *= 0.01f;


            int _count = 1000;
            for (Vector2 _iVec = _start; true; _iVec += _lineVector)
            {
                bool breaked = false;
                _count--;
                if (_count == 0)
                {
                    break;
                }
                for (int i = -1; i <= 1; i++)
                {

                    MyTile _tile = GetTileWithActualPos(_iVec + _lineNormalVector * i);
                    _gizmoOpenList.Add(_tile.actualPos);
                    if (_tile.colliding == true)
                    {
                        return _tile;
                    }
                    if (_tile.logicalPos == _destLogicalPos)
                    {
                        breaked = true;
                        break;
                    }
                }
                if (breaked)
                {
                    break;
                }

            }

            return null;
        }

        MyTile GetFirstCollidedTileJPS(Vector2 _start, Vector2 _dest)
        {

            MyTile _destTile = GetTileWithActualPos(_dest);
            MyTile _startTile = GetTileWithActualPos(_start);
            Vector2Int _destLogicalPos = _destTile.logicalPos;
            Vector2Int _startLogicalPos = _startTile.logicalPos;
            Vector2 _lineVector = _destLogicalPos - _startLogicalPos;
            Debug.Log(_lineVector);
            if (_lineVector == Vector2.zero)
            {
                return null;
            }

            //_changer ;
            if (Mathf.Abs(_lineVector.x) > Mathf.Abs(_lineVector.y))
            {
                float _x = _lineVector.x;
                float _y = _lineVector.y;
                if (_x < 0)
                {
                    _lineVector.x = -1;
                }
                else
                {
                    _lineVector.x = 1;
                }

                _lineVector.y = _y / _x;
            }
            else if (Mathf.Abs(_lineVector.x) > Mathf.Abs(_lineVector.y))
            {

                float _x = _lineVector.x;
                float _y = _lineVector.y;
                _lineVector.x = _x / _y;
                if (_y < 0)
                {
                    _lineVector.y = -1;
                }
                else
                {
                    _lineVector.y = 1;
                }
            }
            else
            {
                if (_lineVector.x < 0)
                {
                    _lineVector.x = -1;
                }
                else
                {
                    _lineVector.x = 1;
                }
                if (_lineVector.y < 0)
                {
                    _lineVector.y = -1;
                }
                else
                {
                    _lineVector.y = 1;
                }
            }
            Debug.Log(_lineVector);
            Vector2 _changer = new Vector2(-_lineVector.y, _lineVector.x) * 0.7f;
            for (Vector2 _iVec = _startLogicalPos;
                _iVec.x != _destLogicalPos.x && _iVec.y != _destLogicalPos.y;
                 _iVec += _lineVector)
            {

                for (int i = -1; i <= 1; i++)
                {
                    Vector2Int _logicalPos = new Vector2Int((int)_iVec.x + Mathf.RoundToInt(_changer.x) * i,
                        (int)_iVec.y + Mathf.RoundToInt(_changer.y) * i);

                    MyTile _tile = _tileDictionary[_logicalPos];
                    _gizmoOpenList.Add(_tile.actualPos);
                    if (_tile.colliding == true)
                    {
                        return _tile;
                    }

                }
            }

            return null;
        }

        IEnumerator MoveToPath(PathNode _path)
        {
            bool _toggle = _pathFindingToggle;
            float _timer = 0;
            PathNode _nowPath = _path;
            List<PathNode> _pathList = new List<PathNode>();
            float _speed = 0;

            int _pathIndex = 0;
            _pathFinding = true;
            while (_nowPath != null)
            {
                _pathList.Add(_nowPath);
                _nowPath = _nowPath.parentNode;
            }
            _pathIndex = _pathList.Count - 1;
            while (_pathFindingToggle == _toggle)
            {
                Vector2 _start;
                Vector2 _end;



                if (_pathIndex < 1)
                {
                    break;
                }
                _start = _pathList[_pathIndex].actualPos;
                _end = _pathList[_pathIndex - 1].actualPos;
                if (_speedIndex >= _speedArray.Length)
                {

                    _speedIndex = 0;
                }
                _speed = _speedArray[_speedIndex] / ((_pathList[_pathIndex - 1].tile.logicalPos - _pathList[_pathIndex].tile.logicalPos).magnitude);
                while (_pathFindingToggle == _toggle)
                {

                    _timer += Time.deltaTime * _speed;
                    _speedTimer += Time.deltaTime;
                    _playerObject.transform.position = Vector2.Lerp(_start, _end, _timer);
                    yield return null;
                    if (_timer > 1)
                    {
                        _timer = 0;
                        break;
                    }
                    if (_speedTimer > 0.2f)
                    {
                        _speedTimer = 0;
                        _speedIndex++;
                        if (_speedIndex >= _speedArray.Length)
                        {

                            _speedIndex = 0;
                        }
                        _speed = _speedArray[_speedIndex] / ((_pathList[_pathIndex - 1].tile.logicalPos - _pathList[_pathIndex].tile.logicalPos).magnitude);

                    }
                }
                _pathIndex--;


            }
            _pathFinding = false;
        }

        private void OnDrawGizmos()
        {
            if (_gizmoOpenList != null)
            {
                Gizmos.color = Color.red;
                for (int i = 0; i < _gizmoOpenList.Count; i++)
                {
                    Gizmos.DrawCube(_gizmoOpenList[i], new Vector3(0.1f, 0.1f, 0));
                }
            }
            if (_gizmoClosedList != null)
            {
                Gizmos.color = Color.blue;
                for (int i = 0; i < _gizmoClosedList.Count; i++)
                {
                    Gizmos.DrawCube(_gizmoClosedList[i], new Vector3(0.1f, 0.1f, 0));
                }
            }
            Gizmos.color = Color.green;
            Gizmos.DrawCube(_gizmoMouse, new Vector3(0.1f, 0.1f, 0));
            Gizmos.color = Color.magenta;
            Gizmos.DrawCube(_gizmoTile, new Vector3(0.1f, 0.1f, 0));

        }

    }

}