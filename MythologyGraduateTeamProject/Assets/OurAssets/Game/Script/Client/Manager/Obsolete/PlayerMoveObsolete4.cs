using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Obsolete
{

    public class PlayerMoveObsolete4 : MonoBehaviour
    {

        enum KeyboardArrow
        {
            Left, Down, Up, Right
        }
        enum Direction
        {
            Left, DownLeft, Down, DownRight, Right, UpRight, Up, UpLeft
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
        private List<Vector2> _gizmoLineStartList;
        private List<Vector2> _gizmoLineDirList;
        private Vector2 _gizmoMouse;
        private Vector2 _gizmoTile;
        private int PATH_FINDING_RANGE;
        private int DIRECTION_ENUM_LENGTH;


        // Start is called before the first frame update
        void Start()
        {
            _gizmoOpenList = new List<Vector2>();
            _gizmoClosedList = new List<Vector2>();
            _gizmoLineStartList = new List<Vector2>();
            _gizmoLineDirList = new List<Vector2>();
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
            DIRECTION_ENUM_LENGTH = System.Enum.GetValues(typeof(Direction)).Length;


        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                SetKeyboardMove(KeyboardArrow.Down, true);
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                SetKeyboardMove(KeyboardArrow.Left, true);
            }
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                SetKeyboardMove(KeyboardArrow.Right, true);
            }
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                SetKeyboardMove(KeyboardArrow.Up, true);
            }

            if (Input.GetKeyUp(KeyCode.DownArrow))
            {
                SetKeyboardMove(KeyboardArrow.Down, false);
            }
            if (Input.GetKeyUp(KeyCode.LeftArrow))
            {
                SetKeyboardMove(KeyboardArrow.Left, false);
            }
            if (Input.GetKeyUp(KeyCode.RightArrow))
            {
                SetKeyboardMove(KeyboardArrow.Right, false);
            }
            if (Input.GetKeyUp(KeyCode.UpArrow))
            {
                SetKeyboardMove(KeyboardArrow.Up, false);
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

        private List<Direction> GetAdjacentCollidingTiles(Vector2Int _tileLogicalPos)
        {
            List<Direction> _collidingDirectionList = new List<Direction>();
            Vector2Int _logicalPos = Vector2Int.zero;
            for (int i = 0; i < 8; i++)
            {
                Direction _dir = (Direction)i;
                _logicalPos = GetLogicVectorWithDirectionEnum(_dir);
                MyTile _tile = _tileDictionary[_tileLogicalPos + _logicalPos];
                if (_tile.colliding == true)
                {
                    _collidingDirectionList.Add(_dir);
                }
            }

            return _collidingDirectionList;

        }


        private MyTile GetTileWithActualPos(Vector2 _pos)
        {
            return _tileDictionary[MyTile.ActualToLogicalPosition(_pos)];
        }

        void SetKeyboardMove(KeyboardArrow _keyboardArrow, bool _pressing)
        {
            _keyboardMoving = false;
            _keyMoveArray[(int)_keyboardArrow] = _pressing;
            _moveDirection = Vector2.zero;
            for (int i = 0; i < _keyMoveArray.Length; i++)
            {
                if (_keyMoveArray[i] == true)
                {
                    _keyboardMoving = true;
                    switch ((KeyboardArrow)i)
                    {
                        case KeyboardArrow.Down:
                            _moveDirection += Vector2.down;
                            break;
                        case KeyboardArrow.Left:
                            _moveDirection += Vector2.left;
                            break;
                        case KeyboardArrow.Right:
                            _moveDirection += Vector2.right;
                            break;
                        case KeyboardArrow.Up:
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
            _gizmoLineDirList.Clear();
            _gizmoLineStartList.Clear();
            Vector2 _endPos = GetTileWithActualPos(_mouseWorldPos).actualPos;
            _gizmoLineStartList.Add(_playerObject.transform.position);
            _gizmoLineDirList.Add(_mouseWorldPos - (Vector2)_playerObject.transform.position);
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

        bool CheckInPathRange(Vector2Int _aPos, Vector2Int _bPos)
        {
            return Mathf.Abs(_aPos.x - _bPos.x) < PATH_FINDING_RANGE && Mathf.Abs(_aPos.y - _bPos.y) < PATH_FINDING_RANGE;
        }

        PathNode GetShortestPathJPS(Vector2 _start, Vector2 _end)
        {
            MyTile _startTile = GetTileWithActualPos(_start);

            MyTile _endTile = GetTileWithActualPos(_end);
            _gizmoTile = _endTile.actualPos;
            if (_endTile.colliding == true || !CheckInPathRange(_startTile.logicalPos, _endTile.logicalPos))
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
            Direction _nowGoing = Direction.UpRight;


            while (true)
            {
                Vector2Int _nowTracingTilePos = _nowPath.tile.logicalPos;
                bool _collided = false;
                GetFirstCollidedTileInActualLine(_nowStartPos, _endTile.actualPos, out _collided);
                if (_collided == false)
                {
                    PathNode _openPath = new PathNode(_endTile.actualPos, _endTile, _nowPath,
                            0, _nowPath.gValue + (_endTile.actualPos - _nowStartPos).magnitude);
                    _gizmoOpenList.Add(_openPath.tile.actualPos);
                    _openPathList.Add(_openPath);
                }
                else
                {
                    for (int i = 1; i < 8; i += 2)
                    {
                        _nowGoing = (Direction)i;
                        while (true)
                        { //2간격으로 해야 대각선 아니고 직선이 나옴.
                            for (int j = -1; j <= 1; j += 2)
                            {
                                _gValue = 0;
                                Direction _nowStraightLineDirection = Direction.Left;
                                if ((int)_nowGoing + j == DIRECTION_ENUM_LENGTH)
                                {
                                    _nowStraightLineDirection = (Direction)0;
                                }
                                else
                                {
                                    _nowStraightLineDirection = (Direction)((int)_nowGoing + j);
                                }
                                Vector2Int _straightLineDir = GetLogicVectorWithDirectionEnum(_nowStraightLineDirection);
                                for (Vector2Int _iVec = _nowTracingTilePos; CheckInPathRange(_iVec, _nowStandTile.logicalPos); _iVec += _straightLineDir)
                                {
                                    Vector2 _walkingValue = _iVec - _nowTracingTilePos;
                                    _walkingValue.x *= MyTile.TILE_X_SIZE;
                                    _walkingValue.y *= MyTile.TILE_Y_SIZE;
                                    _gValue = _walkingValue.magnitude;
                                    if (_tileDictionary[_iVec] == _endTile)
                                    {
                                        PathNode _openPath = null;
                                        for (int n = 0; n < _openPathList.Count; n++)
                                        {
                                            if (_openPathList[n].tile == _endTile)
                                            {
                                                _openPath = _openPathList[n];
                                                break;
                                            }
                                        }
                                        if (_openPath == null)
                                        {
                                            _openPath = new PathNode(_endTile.actualPos, _endTile, _nowPath,
                                                0, _nowPath.gValue + _gValue);
                                            _gizmoOpenList.Add(_openPath.tile.actualPos);
                                            _openPathList.Add(_openPath);
                                            //_gizmoOpenList.Add(_openPath.actualPos);
                                        }
                                        else
                                        {
                                            _openPath.UpdateValues(0, _gValue);
                                        }
                                    }
                                }

                                bool _colliding = false;
                                GetCollidedTileInStraightLine(_nowTracingTilePos, _nowStraightLineDirection, out _colliding);
                                if (_colliding == true)
                                {
                                    continue;
                                }
                                for (int k = -1; k <= 1; k++)
                                {
                                    bool _otherLineColliding = false;
                                    List<MyTile> _collidingTileList;
                                    int _nowDirectionInt = ((int)_nowStraightLineDirection + k * 2) % DIRECTION_ENUM_LENGTH;
                                    if (_nowDirectionInt < 0)
                                    {
                                        _nowDirectionInt += DIRECTION_ENUM_LENGTH;
                                    }
                                    //한칸 쓰윽 가고 좌우로 가서 체크.
                                    Vector2Int nowLookingRow = _nowTracingTilePos
                                        + GetLogicVectorWithDirectionEnum((Direction)_nowDirectionInt);
                                    _collidingTileList = GetCollidedTilesInStraightLine(nowLookingRow, _nowStraightLineDirection, out _otherLineColliding);
                                    if (_otherLineColliding == false)
                                    {
                                        continue;
                                    }

                                    for (int m = 0; m < _collidingTileList.Count; m++)
                                    {
                                        MyTile _collideTile = _collidingTileList[m];

                                        Vector2Int _collideTileLogicalPos = _collideTile.logicalPos;
                                        Vector2Int _nextDir = GetLogicVectorWithDirectionEnum(_nowStraightLineDirection);
                                        Vector2Int _pathNodeLogicalPos = _collideTileLogicalPos + _nextDir;
                                        //프리라인 들어가야함

                                        Vector2 _walkingValue = _nowTracingTilePos - _collideTileLogicalPos;
                                        _walkingValue.x *= MyTile.TILE_X_SIZE;
                                        _walkingValue.y *= MyTile.TILE_Y_SIZE;
                                        _gValue = _walkingValue.magnitude;
                                        MyTile _nowTile = _tileDictionary[_pathNodeLogicalPos];
                                        if (_nowTile.colliding == true)
                                        {
                                            continue;
                                        }
                                        PathNode _openPath = null;
                                        for (int n = 0; n < _openPathList.Count; n++)
                                        {
                                            if (_openPathList[n].tile == _nowTile)
                                            {
                                                _openPath = _openPathList[n];
                                                break;
                                            }
                                        }
                                        if (_openPath == null)
                                        {
                                            _openPath = new PathNode(_nowTile.actualPos, _nowTile, _nowPath,
                                                (_endTile.actualPos - _nowTile.actualPos).magnitude, _nowPath.gValue + _gValue);
                                            _openPathList.Add(_openPath);
                                            _gizmoOpenList.Add(_openPath.tile.actualPos);
                                            //_gizmoOpenList.Add(_openPath.actualPos);
                                        }
                                        else
                                        {
                                            _openPath.UpdateValues((_endTile.actualPos - _nowTile.actualPos).magnitude, _nowPath.gValue + _gValue);
                                        }
                                    }
                                }

                            }
                            Vector2Int _nextDiagonalPos = _nowTracingTilePos +
                                GetLogicVectorWithDirectionEnum(_nowGoing);
                            if (CheckInPathRange(_nowStandTile.logicalPos, _nextDiagonalPos) == true)
                            {
                                bool _blocked = false;
                                for (int j = -1; j <= 1; j += 2)
                                {
                                    int _dirNum = (int)_nowGoing + i;
                                    _dirNum %= DIRECTION_ENUM_LENGTH;
                                    if (_dirNum < 0)
                                    {
                                        _dirNum += DIRECTION_ENUM_LENGTH;
                                    }
                                    _nextDiagonalPos = _nowTracingTilePos + GetLogicVectorWithDirectionEnum(_nowGoing);
                                    if (_tileDictionary[_nextDiagonalPos].colliding == true)
                                    {
                                        _blocked = true;
                                        break;
                                    }
                                }
                                if (_blocked == true)
                                {
                                    break;
                                }
                                _nowTracingTilePos = _nextDiagonalPos;
                            }
                            else
                            {
                                break;
                            }
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
                _gizmoClosedList.Add(_nowPath.actualPos);
                if (_nowPath.tile == _endTile)
                {
                    return _nowPath;
                }

            }
        }


        MyTile GetCollidedTileInStraightLine(Vector2Int _startLogicalPos, Direction _direction, out bool _colliding)
        {
            Vector2Int _directionVector = GetLogicVectorWithDirectionEnum(_direction);

            for (Vector2Int _iVec = _startLogicalPos;
                CheckInPathRange(_iVec, _nowStandTile.logicalPos);
                _iVec += _directionVector)
            {
                if (_tileDictionary[_iVec].colliding == true)
                {
                    _colliding = true;
                    return _tileDictionary[_iVec];
                }
            }
            _colliding = false;
            return null;
        }

        List<MyTile> GetCollidedTilesInStraightLine(Vector2Int _startLogicalPos, Direction _direction, out bool _colliding)
        {
            List<MyTile> _tileList = new List<MyTile>();
            Vector2Int _directionVector = GetLogicVectorWithDirectionEnum(_direction);
            _colliding = false;
            for (Vector2Int _iVec = _startLogicalPos;
                CheckInPathRange(_iVec, _nowStandTile.logicalPos);
                _iVec += _directionVector)
            {
                if (_tileDictionary[_iVec].colliding == true)
                {
                    _colliding = true;
                    _tileList.Add(_tileDictionary[_iVec]);
                }
            }

            return _tileList;
        }

        List<MyTile> GetTilesInStraightLine(Vector2Int _startLogicalPos, Direction _direction)
        {
            List<MyTile> _tileList = new List<MyTile>();
            Vector2Int _directionVector = GetLogicVectorWithDirectionEnum(_direction);

            for (Vector2Int _iVec = _startLogicalPos;
                _iVec.y - _nowStandTile.logicalPos.x <= PATH_FINDING_RANGE
                && _iVec.y - _nowStandTile.logicalPos.y <= PATH_FINDING_RANGE;
                _iVec += _directionVector)
            {
                _tileList.Add(_tileDictionary[_iVec]);
            }

            return _tileList;
        }
        MyTile GetFirstCollidedTileInActualLine(Vector2 _startActualPos, Vector2 _endActualPos, out bool _colliding)
        {
            _gizmoOpenList.Clear();

            MyTile _tile = null;

            MyTile _startTile = GetTileWithActualPos(_startActualPos);
            MyTile _endTile = GetTileWithActualPos(_endActualPos);
            Vector2 _line = _endActualPos - _startActualPos;
            Vector2 _aStart = _startActualPos;
            Vector2 _aEnd = _endActualPos;
            Vector2 _aDir = _line;
            if (_startTile.colliding == true)
            {
                _colliding = true;
                return _startTile;
            }

            List<Vector2> _linePosList;
            Vector2 _xLineShift;
            Vector2 _yLineShift;
            Vector2 _xBStart;
            Vector2 _xBDir;

            Vector2 _yBStart;
            Vector2 _yBDir;

            _colliding = false;
            if (_aDir == Vector2.zero)
            {
                _colliding = false;
                return null;
            }
            //_aDir.Normalize();
            //left로가는거 먼저하자.
            if (_aDir.x > 0)
            {
                _xLineShift = new Vector2(MyTile.TILE_X_SIZE, 0);
                if (_aDir.y > 0)
                {
                    _yLineShift = new Vector2(0, MyTile.TILE_Y_SIZE);
                    _xBDir = MyTile.LEFT_UP_VECTOR;
                    _yBDir = MyTile.RIGHT_UP_VECTOR;
                }
                else
                {
                    _yLineShift = new Vector2(0, -MyTile.TILE_Y_SIZE);
                    _xBDir = MyTile.LEFT_DOWN_VECTOR;
                    _yBDir = MyTile.RIGHT_DOWN_VECTOR;
                }
            }
            else
            {
                _xLineShift = new Vector2(-MyTile.TILE_X_SIZE, 0);
                if (_aDir.y > 0)
                {
                    _yLineShift = new Vector2(0, MyTile.TILE_Y_SIZE);
                    _xBDir = MyTile.RIGHT_UP_VECTOR;
                    _yBDir = MyTile.LEFT_UP_VECTOR;
                }
                else
                {
                    _yLineShift = new Vector2(0, -MyTile.TILE_Y_SIZE);
                    _xBDir = MyTile.RIGHT_DOWN_VECTOR;
                    _yBDir = MyTile.LEFT_DOWN_VECTOR;
                }
            }
            _xBStart = _startTile.actualPos - _xLineShift / 2;
            _yBStart = _startTile.actualPos - _yLineShift / 2;
            _linePosList = new List<Vector2>();


            _linePosList.Add(_startActualPos);
            while (true)
            {
                bool _xIntersecting = false;
                Vector2 _xIntersect = GetAFiniteBInfiniteLineIntersect(_startActualPos, _endActualPos, _xBStart, _xBDir, out _xIntersecting);
                if (_xIntersecting == true)
                {

                    _linePosList.Add(_xIntersect);
                }



                bool _y1Intersecting = false;
                Vector2 _y1Intersect = GetAFiniteBInfiniteLineIntersect(_startActualPos, _endActualPos, _yBStart, _yBDir, out _y1Intersecting);
                if (_y1Intersecting == true)
                {

                    _linePosList.Add(_y1Intersect);
                }

                bool _y2Intersecting = false;
                Vector2 _y2Intersect = GetAFiniteBInfiniteLineIntersect(_startActualPos, _endActualPos, _xBStart, _yBDir, out _y2Intersecting);
                if (_y2Intersecting == true)
                {

                    _linePosList.Add(_y2Intersect);
                }

                if (!_y2Intersecting && !_y1Intersecting && !_xIntersecting)
                {
                    break;
                }

                _xBStart += _xLineShift;
                _yBStart += _yLineShift;
            }
            _linePosList.Add(_endActualPos);



            for (int i = 0; i < _linePosList.Count - 1; i++)
            {
                Vector2 _start = _linePosList[i];
                Vector2 _end = _linePosList[i + 1];
                Vector2 _middle = (_end + _start) / 2f;



                if (_start == _end)
                {
                    List<MyTile> _tileList = GetCollidingTilesInRadius(_start);
                    for (int j = 0; j < _tileList.Count; j++)
                    {
                        if (_tileList[j].colliding == true)
                        {
                            _colliding = true;
                            return _tileList[j];
                        }
                    }
                }
                else
                {
                    _tile = GetTileWithActualPos(_middle);
                    if (_tile.colliding == true)
                    {
                        _colliding = true;
                        return _tile;
                    }
                }

            }
            if (_endTile.colliding == true)
            {
                _colliding = true;
                return _endTile;
            }
            _colliding = false;
            return null;
        }


        Vector2 GetInfiniteLineIntersect(Vector2 _aStart, Vector2 _aDir, Vector2 _bStart, Vector2 _bDir, out bool _intersecting)
        {
            float _bT = (_aDir.x * _aStart.y + _bStart.x * _aStart.y - _bStart.y * _aDir.x) /
        (_aDir.x * _bDir.y - _aDir.y * _bDir.x);
            if (_bT >= 0)
            {
                _intersecting = true;
            }
            else
            {
                _intersecting = false;
                return Vector2.zero;
            }
            Vector2 _intersect = _bStart + _bDir * _bT;
            return _intersect;
        }

        Vector2 GetFiniteLineIntersect(Vector2 _aStart, Vector2 _aEnd, Vector2 _bStart, Vector2 _bEnd, out bool _intersecting)
        {
            Vector2 _aDir = _aEnd - _aStart;
            if (_aDir == Vector2.zero)
            {
                _intersecting = false;
                return Vector2.zero;
            }
            _aDir.Normalize();
            float _aTLimit = (_aEnd - _aStart).magnitude;

            Vector2 _bDir = _bEnd - _bStart;
            if (_bDir == Vector2.zero)
            {
                _intersecting = false;
                return Vector2.zero;
            }
            _bDir.Normalize();
            float _bTLimit = (_bEnd - _bStart).magnitude;


            float _bT = (_aDir.x * _aStart.y + _bStart.x * _aStart.y - _bStart.y * _aDir.x) /
        (_aDir.x * _bDir.y - _aDir.y * _bDir.x);
            if (_bT >= 0 && _bT <= _bTLimit)
            {
                _intersecting = true;
                Vector2 _intersect = _bStart + _bDir * _bT;
                return _intersect;
            }
            else
            {
                _intersecting = false;
                return Vector2.zero;
            }
        }

        Vector2 GetAFiniteBInfiniteLineIntersect(Vector2 _aStart, Vector2 _aEnd, Vector2 _bStart, Vector2 _bDir, out bool _intersecting)
        {
            Vector2 _aDir = _aEnd - _aStart;
            if (_aDir == Vector2.zero)
            {
                _intersecting = false;
                return Vector2.zero;
            }
            _gizmoLineStartList.Add(_bStart);
            _gizmoLineDirList.Add(_bDir);
            float _aTLimit = 1;

            float _aT = (_bDir.y * (_aStart.x - _bStart.x) + _bDir.x * (_bStart.y - _aStart.y)) /
        (_aDir.y * _bDir.x - _bDir.y * _aDir.x);
            if (_aT >= 0 && _aT <= _aTLimit)
            {
                _intersecting = true;
            }
            else
            {
                _intersecting = false;
                return Vector2.zero;
            }
            _intersecting = true;
            Vector2 _intersect = _aStart + _aDir * _aT;
            return _intersect;
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

            if (_gizmoLineStartList != null && _gizmoLineDirList != null)
            {

                Gizmos.color = Color.grey;
                for (int i = 0; i < _gizmoLineStartList.Count; i++)
                {
                    Gizmos.DrawLine(_gizmoLineStartList[i], _gizmoLineStartList[i] + 10 * _gizmoLineDirList[i]);
                }
            }
            Gizmos.color = Color.green;
            Gizmos.DrawCube(_gizmoMouse, new Vector3(0.1f, 0.1f, 0));
            Gizmos.color = Color.magenta;
            Gizmos.DrawCube(_gizmoTile, new Vector3(0.1f, 0.1f, 0));

        }

        Vector2Int GetLogicVectorWithDirectionEnum(Direction _dir)
        {
            Vector2Int _vec = Vector2Int.zero;
            switch (_dir)
            {
                case Direction.Left:
                    _vec = new Vector2Int(-1, 0);
                    break;
                case Direction.DownLeft:
                    _vec = new Vector2Int(-1, -1);
                    break;
                case Direction.Down:
                    _vec = new Vector2Int(0, -1);
                    break;
                case Direction.DownRight:
                    _vec = new Vector2Int(1, -1);
                    break;
                case Direction.Right:
                    _vec = new Vector2Int(1, 0);
                    break;
                case Direction.UpRight:
                    _vec = new Vector2Int(1, 1);
                    break;
                case Direction.Up:
                    _vec = new Vector2Int(0, 1);
                    break;
                case Direction.UpLeft:
                    _vec = new Vector2Int(-1, 1);
                    break;
            }
            return _vec;
        }

    }


}