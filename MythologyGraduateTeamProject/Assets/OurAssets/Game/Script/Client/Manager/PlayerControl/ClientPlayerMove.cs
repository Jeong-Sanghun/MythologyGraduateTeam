using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum KeyboardArrow
    {
        Left, Down, Up, Right
    }

public enum Direction
{
    Left, DownLeft, Down, DownRight, Right, UpRight, Up, UpLeft
}


public class ClientPlayerMove : PlayerMove
{ 
    [SerializeField]
    private TileMapGenerator _tileMapGenerator;
    [SerializeField]
    private PlayerAnimation _playerAnimation;
    [SerializeField]
    private Camera cam;

    

    
    private Rigidbody2D _playerRigid;
    


    private Vector2 _moveDirection;
    private Vector2 _originMoveDirectionNormalized;


    private bool[] _keyMoveArray;
    public MyTile nowStandTile;    //히;;히

    private const float _playerColliderRadius = 0.4f;
    private float _speedTimer = 0;
    private float _mouseRightClickMaintainTime = 0;
    private bool _keyboardMoving;
    private bool _mouseMoving;
    private bool _pathFinding;
    private bool _pathFindingToggle;
    //private List<Vector2> _gizmoOpenList;
    //private List<Vector2> _gizmoClosedList;
    //private List<Vector2> _gizmoLineStartList;
    //private List<Vector2> _gizmoLineDirList;
    //private Vector2 _gizmoMouse;
    //private Vector2 _gizmoTile;
    private int PATH_FINDING_RANGE;
    public static int DIRECTION_ENUM_LENGTH;

    
    


    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();


        _playerRigid = _characterObject.GetComponent<Rigidbody2D>();

        _keyboardMoving = false;
        _mouseMoving = false;
        _pathFinding = false;
        _pathFindingToggle = true;
        _keyMoveArray = new bool[4];
        for (int i = 0; i < _keyMoveArray.Length; i++)
        {
            _keyMoveArray[i] = false;
        }
        
        PATH_FINDING_RANGE = Constants.PATH_FINDING_RANGE;
        DIRECTION_ENUM_LENGTH = System.Enum.GetValues(typeof(Direction)).Length;

        //Vector2Int _randomStartLogicalPos = new Vector2Int(Random.Range(0,TileMapDataGenerator.MAP_X_SIZE * TileMapDataGenerator.ZONE_X_SIZE - 20),
        //Random.Range(0,TileMapDataGenerator.MAP_Y_SIZE * TileMapDataGenerator.ZONE_Y_SIZE - 20));
        Vector2Int _randomStartLogicalPos = new Vector2Int(30, 30);
        _characterObject.transform.position = MyTile.LogicalToActualPos(_randomStartLogicalPos);
        _tileMapGenerator.OnMapChange(_randomStartLogicalPos);
        _tileDictionary = _tileMapGenerator.tileDictionary;
        nowStandTile = _tileDictionary[_randomStartLogicalPos];
        OnMapChange();
    }

    // Update is called once per frame
    public override void OnFrameChange()
    {
        base.OnFrameChange();

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

    public List<MyTile> GetCollidingTilesInRadius()
    {
        Vector2 _pos = _characterObject.transform.position;
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
        Vector2Int _logicalPos = MyTile.ActualToLogicalPosition(_pos);
        if (_logicalPos.x < 0)
        {
            _logicalPos.x = 0;
        }
        if (_logicalPos.y < 0)
        {
            _logicalPos.y = 0;
        }

        if (_logicalPos.x >= TileMapDataGenerator.MAP_X_SIZE * TileMapDataGenerator.ZONE_X_SIZE)
        {
            _logicalPos.x = 0;
        }
        if (_logicalPos.y >= TileMapDataGenerator.MAP_Y_SIZE * TileMapDataGenerator.ZONE_Y_SIZE)
        {
            _logicalPos.y = 0;
        }


        return _tileDictionary[_logicalPos];
    }


    public void SetKeyboardMove(KeyboardArrow _keyboardArrow, bool _pressing)
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
            _playerAnimation.SetAnimation(PlayerAnimationEnum.Run, true);
            _mouseMoving = false;
            _pathFinding = false;
            _pathFindingToggle = !_pathFindingToggle;
        }
        _originMoveDirectionNormalized = _moveDirection;
        _originMoveDirectionNormalized.Normalize();

        if (_moveDirection == Vector2.zero)
        {
            _playerAnimation.SetAnimation(PlayerAnimationEnum.Run, false);
            _speedTimer = 0;
            _playerRigid.velocity = Vector2.zero;
        }
        else
        {
            float _angle = Vector2.Dot(Vector2.up,_originMoveDirectionNormalized);
            if (_originMoveDirectionNormalized.x < 0)
            {
               _modelObject.transform.localEulerAngles = new Vector3(0, -180 * Mathf.Acos(_angle) /Mathf.PI, 0);
            }
            else{
                _modelObject.transform.localEulerAngles = new Vector3(0, 180 * Mathf.Acos(_angle) /Mathf.PI, 0);
            }
            _playerAnimation.SetAnimation(PlayerAnimationEnum.Run, true);
        }
    }

    

    public void OnMouseRightClickDown(Vector2 _mouse_canvas_pos)
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
        _mouseRightClickMaintainTime = 0;
        Vector2 _endPos = GetTileWithActualPos(_mouseWorldPos).actualPos;
        
        
        PathNode _path = GetShortestPathJPS(_characterObject.transform.position, _endPos);
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
            Vector2 _direction = _endPos - (Vector2)_characterObject.transform.position;
            _direction.Normalize();
            _originMoveDirectionNormalized = _direction;
        }
    }

    public void Move()
    {
        if (_pathFinding == true)
        {
            return;
        }
       
        if (_speedTimer < 1.2f)
        {
            _speedTimer += Time.deltaTime;
        }
        _playerRigid.velocity = _originMoveDirectionNormalized * _speedTimer*2;
        nowStandTile = GetTileWithActualPos(_characterObject.transform.position);
        List<MyTile> _tile_List = GetCollidingTilesInRadius((Vector2)_characterObject.transform.position);
        for (int i = 0; i < _tile_List.Count; i++)
        {
            _dataCenter.tilemapGenerator.SetColliderObject(_tile_List[i]);
        }

    }

    public void Decollide()
    {
        _dataCenter.tilemapGenerator.SetActiveFalseCollider();

        nowStandTile = GetTileWithActualPos(_characterObject.transform.position);
        List<MyTile> _tile_List = GetCollidingTilesInRadius((Vector2)_characterObject.transform.position);
        for (int i = 0; i < _tile_List.Count; i++)
        {
            _dataCenter.tilemapGenerator.SetColliderObject(_tile_List[i]);
        }
    }

    bool CheckInPathRange(Vector2Int _aPos, Vector2Int _bPos)
    {
        return Mathf.Abs(_aPos.x - _bPos.x) <= PATH_FINDING_RANGE && Mathf.Abs(_aPos.y - _bPos.y) <= PATH_FINDING_RANGE;
    }

    bool CheckInPathRange(Vector2 _aPos, Vector2 _bPos)
    {
        return Mathf.Abs(_aPos.x - _bPos.x) <= PATH_FINDING_RANGE*MyTile.TILE_X_SIZE && Mathf.Abs(_aPos.y - _bPos.y) <= PATH_FINDING_RANGE*MyTile.TILE_Y_SIZE;
    }

    PathNode GetShortestPathJPS(Vector2 _start, Vector2 _end)
    {
        MyTile _startTile = GetTileWithActualPos(_start);

        MyTile _endTile = GetTileWithActualPos(_end);
;
        if (_endTile.colliding == true)
        {
            return null;
        }
        PathNode _returnPath = new PathNode(_start, _startTile, null, (_endTile.actualPos - _start).magnitude, 0);
        List<PathNode> _openPathList = new List<PathNode>();
        List<PathNode> _closePathList = new List<PathNode>();
        PathNode _nowPath = _returnPath;
        _closePathList.Add(_nowPath);

        Vector2 _nowStartPos = _start;
        float _gValue = 0;


        while (true)
        {

            _nowStartPos = _nowPath.actualPos;
            bool _colliding = false;
            MyTile _collidedTile = GetFirstCollidedTileInActualLine(_nowStartPos, _end,out _colliding);
            if (_colliding==false)
            {
                PathNode _openPath = new PathNode(_endTile.actualPos, _endTile, _nowPath,
                    0, _nowPath.gValue + (_endTile.actualPos - _nowStartPos).magnitude);
                _openPathList.Add(_openPath);

            }
            else
            {

                Vector2Int _logicalPos = _collidedTile.logicalPos;
                List<MyTile> _nodeTileList = GetAdjacentCollideTiles(_collidedTile);
                if (_nodeTileList.Count == 0)
                {

                    break;
                }
                for (int i = 0; i < _nodeTileList.Count; i++)
                {

                    MyTile _nowTile = _nodeTileList[i];
                    
                    if (_nowTile.colliding == true)
                    {
                        continue;
                    }
                    if (!CheckInPathRange(_nowTile.logicalPos, nowStandTile.logicalPos))
                    {
                        continue;
                    }
                    bool _collidingIn = false;
                    MyTile _collidedTileIn = GetFirstCollidedTileInActualLine(_nowStartPos, _nowTile.actualPos, out _collidingIn);
                    if (_collidingIn == true)
                    {
                        continue;

                    }
                    _gValue = (_nowStartPos - _nowTile.actualPos).magnitude;
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
                            (_endTile.actualPos - _nowTile.actualPos).magnitude, _nowPath.gValue + _gValue);
                        _openPathList.Add(_openPath);

                    }
                    else
                    {
                        _openPath.UpdateValues((_endTile.actualPos - _nowTile.actualPos).magnitude, _nowPath.gValue + _gValue, _nowPath);
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
        return null;
    }

    public List<MyTile> GetAdjacentCollideTiles(MyTile _givenTile)
    {
        List<MyTile> _tileList = new List<MyTile>();
        List<MyTile> _collideTileList = new List<MyTile>();
        _collideTileList.Add(_givenTile);
        int _nowCollideTileListIndex = 0;
        while (true)
        {
            if (_nowCollideTileListIndex >= _collideTileList.Count)
            {
                break;
            }
            MyTile _nowTile = _collideTileList[_nowCollideTileListIndex];
            _nowCollideTileListIndex++;
            Vector2Int _logicalPos = _nowTile.logicalPos;

            for (int i = 0; i < DIRECTION_ENUM_LENGTH; i++)
            {
                Vector2Int _tracingPos = _logicalPos + GetLogicVectorWithDirectionEnum((Direction)i);
                if (!CheckInPathRange(_tracingPos, nowStandTile.logicalPos))
                {
                    continue;
                }

                MyTile _traceTile = _tileDictionary[_tracingPos];

                if (_traceTile.colliding == true)
                {
                    if (!_collideTileList.Contains(_traceTile))
                    {
                        _collideTileList.Add(_traceTile);
        
                    }
                }
                else
                {
                    if (!_tileList.Contains(_traceTile))
                    {
                        _tileList.Add(_traceTile);
                        //_gizmoOpenList.Add(_traceTile.actualPos);
                    }
                }
            }
        }

        return _tileList;

    }

    MyTile GetCollidedTileInStraightLine(Vector2Int _startLogicalPos, Direction _direction, out bool _colliding)
    {
        Vector2Int _directionVector = GetLogicVectorWithDirectionEnum(_direction);

        for (Vector2Int _iVec = _startLogicalPos;
            CheckInPathRange(_iVec, nowStandTile.logicalPos);
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
            CheckInPathRange(_iVec, nowStandTile.logicalPos);
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
            _iVec.y - nowStandTile.logicalPos.x <= PATH_FINDING_RANGE
            && _iVec.y - nowStandTile.logicalPos.y <= PATH_FINDING_RANGE;
            _iVec += _directionVector)
        {
            _tileList.Add(_tileDictionary[_iVec]);
        }

        return _tileList;
    }
    MyTile GetFirstCollidedTileInActualLine(Vector2 _startActualPos, Vector2 _endActualPos, out bool _colliding)
    {
        //_gizmoOpenList.Clear();
        List<float> _tList;
        MyTile _tile = null;

        MyTile _startTile = GetTileWithActualPos(_startActualPos);
        MyTile _endTile = GetTileWithActualPos(_endActualPos);
        Vector2 _line = _endActualPos - _startActualPos;
        Vector2 _aStart = _startActualPos;
        Vector2 _aEnd = _endActualPos;
        Vector2 _aDir = _line;
        Vector2 _xLineShift;
        Vector2 _yLineShift;
        Vector2 _xBStart;
        Vector2 _xBDir;

        Vector2 _yBStart;
        Vector2 _yBDir;

        Vector2Int _xBLogical;
        Vector2Int _xShiftLogical;
        Vector2Int _yBLogical;
        Vector2Int _yShiftLogical;
        if (_startTile.colliding == true)
        {
            _colliding = true;
            return _startTile;
        }
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
        _xBStart = _startTile.actualPos + _xLineShift/2;
        _yBStart = _startTile.actualPos + _yLineShift/2;
        _tList = new List<float>();
        _tList.Add(0);

        //이거 리스트 안쓰고 제깍제깍 해줘도 됨.

        while (true)
        {

            bool _xIntersecting = false;
            float _t = 0;
            Vector2 _xIntersect = GetAFiniteBInfiniteLineIntersect(_startActualPos, _endActualPos, _xBStart, _xBDir, out _xIntersecting,out _t);
            if (_xIntersecting == true)
            {
                _tList.Add(_t);

            }



            bool _y1Intersecting = false;
            Vector2 _y1Intersect = GetAFiniteBInfiniteLineIntersect(_startActualPos, _endActualPos, _yBStart, _yBDir, out _y1Intersecting, out _t);
            if (_y1Intersecting == true)
            {

                _tList.Add(_t);
 
            }

            bool _y2Intersecting = false;
            Vector2 _y2Intersect = GetAFiniteBInfiniteLineIntersect(_startActualPos, _endActualPos, _xBStart, _yBDir, out _y2Intersecting, out _t);
            if (_y2Intersecting == true)
            {

                _tList.Add(_t);
     
            }

            if (!_y1Intersecting && !_xIntersecting && !_y2Intersecting)
            {
               break;
            }

            //if(Mathf.Abs(_xBStart.x - _startActualPos.x) >Mathf.Abs(_endActualPos.x) &&
            //    Mathf.Abs(_yBStart.y - _startActualPos.y) > Mathf.Abs(_endActualPos.y))
            //{
            //    break;
            //}
            _xBStart += _xLineShift;
            _yBStart += _yLineShift;
            
            
        }
        _tList.Add(1);
        _tList.Sort();
        
        


        for (int i = 0; i < _tList.Count - 1; i++)
        {
            
            Vector2 _start = _startActualPos + _line * _tList[i];
            Vector2 _end = _startActualPos + _line * _tList[i+1];
            Vector2 _middle = (_end + _start) / 2f;



            if ((_end-_start).magnitude<_playerColliderRadius*2)
            {

                List<MyTile> _tileList = GetCollidingTilesInRadius(_middle);
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


    //시작점, 끝점 받아서 사이에 있는 타일들 구하는 함수.
    public List<MyTile> GetIntersectingTiles(Vector2 _startActualPos, Vector2 _endActualPos)
    {
        List<MyTile> _mylist = new List<MyTile>();
        //_gizmoOpenList.Clear();
        List<float> _tList;
        MyTile _tile = null;

        MyTile _startTile = GetTileWithActualPos(_startActualPos);
        MyTile _endTile = GetTileWithActualPos(_endActualPos);
        Vector2 _line = _endActualPos - _startActualPos;
        Vector2 _aStart = _startActualPos;
        Vector2 _aEnd = _endActualPos;
        Vector2 _aDir = _line;
        Vector2 _xLineShift;
        Vector2 _yLineShift;
        Vector2 _xBStart;
        Vector2 _xBDir;

        Vector2 _yBStart;
        Vector2 _yBDir;

        Vector2Int _xBLogical;
        Vector2Int _xShiftLogical;
        Vector2Int _yBLogical;
        Vector2Int _yShiftLogical;


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
        _xBStart = _startTile.actualPos + _xLineShift / 2;
        _yBStart = _startTile.actualPos + _yLineShift / 2;
        _tList = new List<float>();
        _tList.Add(0);

        //이거 리스트 안쓰고 제깍제깍 해줘도 됨.

        while (true)
        {

            bool _xIntersecting = false;
            float _t = 0;
            Vector2 _xIntersect = GetAFiniteBInfiniteLineIntersect(_startActualPos, _endActualPos, _xBStart, _xBDir, out _xIntersecting, out _t);
            if (_xIntersecting == true)
            {
                _tList.Add(_t);

            }



            bool _y1Intersecting = false;
            Vector2 _y1Intersect = GetAFiniteBInfiniteLineIntersect(_startActualPos, _endActualPos, _yBStart, _yBDir, out _y1Intersecting, out _t);
            if (_y1Intersecting == true)
            {

                _tList.Add(_t);

            }

            bool _y2Intersecting = false;
            Vector2 _y2Intersect = GetAFiniteBInfiniteLineIntersect(_startActualPos, _endActualPos, _xBStart, _yBDir, out _y2Intersecting, out _t);
            if (_y2Intersecting == true)
            {

                _tList.Add(_t);

            }

            if (!_y1Intersecting && !_xIntersecting && !_y2Intersecting)
            {
                break;
            }

            //if(Mathf.Abs(_xBStart.x - _startActualPos.x) >Mathf.Abs(_endActualPos.x) &&
            //    Mathf.Abs(_yBStart.y - _startActualPos.y) > Mathf.Abs(_endActualPos.y))
            //{
            //    break;
            //}
            _xBStart += _xLineShift;
            _yBStart += _yLineShift;


        }
        _tList.Add(1);
        _tList.Sort();




        for (int i = 0; i < _tList.Count - 1; i++)
        {

            Vector2 _start = _startActualPos + _line * _tList[i];
            Vector2 _end = _startActualPos + _line * _tList[i + 1];
            Vector2 _middle = (_end + _start) / 2f;



            if ((_end - _start).magnitude < _playerColliderRadius * 2)
            {

                List<MyTile> _tileList = GetCollidingTilesInRadius(_middle);
                for (int j = 0; j < _tileList.Count; j++)
                {
                    if (_mylist.Contains(_tileList[j]) == false)
                    {
                        _mylist.Add(_tileList[j]);
                    }
                }
            }
            else
            {
                _tile = GetTileWithActualPos(_middle);
                if (_mylist.Contains(_tile) == false)
                {
                    _mylist.Add(_tile);
                }
            }
        }
        return _mylist;
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

    Vector2 GetAFiniteBInfiniteLineIntersect(Vector2 _aStart, Vector2 _aEnd, Vector2 _bStart, Vector2 _bDir, out bool _intersecting,out float _t)
    {
        Vector2 _aDir = _aEnd - _aStart;
        if (_aDir == Vector2.zero)
        {
     
            _intersecting = false;
            _t = 0;
            return Vector2.zero;
        }

        float _aTLimit = 1;

        float _aT = (_bDir.y * (_aStart.x - _bStart.x) + _bDir.x * (_bStart.y - _aStart.y)) /
    (_aDir.y * _bDir.x - _bDir.y * _aDir.x);
        
        if (_aT >= 0 && _aT <= _aTLimit)
        {
            _intersecting = true;
            _t = _aT;
            Vector2 _intersect = _aStart + _aDir * _aT;
            return _intersect;
        }
        else
        {
            _t = 0;
            _intersecting = false;
            return Vector2.zero;
        }

    }

    IEnumerator MoveToPath(PathNode _path)
    {
        bool _toggle = _pathFindingToggle;
        float _timer = 0;
        PathNode _nowPath = _path;
        List<PathNode> _pathList = new List<PathNode>();
        float _speed = 0;
        _playerAnimation.SetAnimation(PlayerAnimationEnum.Run, true);
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

            _pathIndex--;


        }
        _pathIndex = _pathList.Count - 1;
        while (_pathFindingToggle == _toggle)
        {
            Vector2 _start;
            Vector2 _end;
            Vector2 _normalVec;



            if (_pathIndex < 1)
            {
                break;
            }
            _start = _pathList[_pathIndex].actualPos;
            _end = _pathList[_pathIndex - 1].actualPos;

            _normalVec = _end-_start;
            _normalVec.Normalize();

            if (_speedIndex >= _speedArray.Length)
            {

                _speed = 2 / ((_pathList[_pathIndex - 1].tile.logicalPos - _pathList[_pathIndex].tile.logicalPos).magnitude);
            }
            else
            {
                _speed = _speedArray[_speedIndex] / ((_pathList[_pathIndex - 1].tile.logicalPos - _pathList[_pathIndex].tile.logicalPos).magnitude);
            }

            float _angle = Vector2.Dot(Vector2.up, _normalVec);
            if (_normalVec.x < 0)
            {
                _modelObject.transform.localEulerAngles = new Vector3(0, -180 * Mathf.Acos(_angle) / Mathf.PI, 0);
            }
            else
            {
                _modelObject.transform.localEulerAngles = new Vector3(0, 180 * Mathf.Acos(_angle) / Mathf.PI, 0);
            }


            while (_pathFindingToggle == _toggle)
            {
              
                _timer += Time.deltaTime * _speed;
                _speedTimer += Time.deltaTime;
                _characterObject.transform.position = Vector2.Lerp(_start, _end, _timer);
                yield return null;
                if (_timer > 1)
                {
                    _timer = 0;
                    break;
                }
                if (_speedTimer > 0.2f)
                {
              
                    if (_speedIndex >= _speedArray.Length)
                    {

                        _speed = 2 / ((_pathList[_pathIndex - 1].tile.logicalPos - _pathList[_pathIndex].tile.logicalPos).magnitude);
                    }
                    else
                    {
                        _speedTimer = 0;
                        _speed = _speedArray[_speedIndex] / ((_pathList[_pathIndex - 1].tile.logicalPos - _pathList[_pathIndex].tile.logicalPos).magnitude);
                        _speedIndex++;
                    }
                }
            }
            if(_pathFindingToggle != _toggle){
                _playerAnimation.SetAnimation(PlayerAnimationEnum.Run, false);
            }
            _pathIndex--;


        }
        _pathFinding = false;
    }

    //private void OnDrawGizmos()
    //{
    //    if (_gizmoOpenList != null)
    //    {
    //        Gizmos.color = Color.red;
    //        for (int i = 0; i < _gizmoOpenList.Count; i++)
    //        {
    //            Gizmos.DrawCube(_gizmoOpenList[i], new Vector3(0.1f, 0.1f, 0));
    //        }
    //    }
    //    if (_gizmoClosedList != null)
    //    {
    //        Gizmos.color = Color.blue;
    //        for (int i = 0; i < _gizmoClosedList.Count; i++)
    //        {
    //            Gizmos.DrawCube(_gizmoClosedList[i], new Vector3(0.1f, 0.1f, 0));
    //        }
    //    }

    //    if (_gizmoLineStartList != null && _gizmoLineDirList != null)
    //    {

    //        Gizmos.color = Color.grey;
    //        for (int i = 0; i < _gizmoLineStartList.Count-1; i++)
    //        {
                
    //            Gizmos.DrawLine(_gizmoLineStartList[i], _gizmoLineStartList[i] + 10 * _gizmoLineDirList[i]);
    //        }
    //    }
    //    Gizmos.color = Color.green;
    //    Gizmos.DrawCube(_gizmoMouse, new Vector3(0.1f, 0.1f, 0));
    //    Gizmos.color = Color.magenta;
    //    Gizmos.DrawCube(_gizmoTile, new Vector3(0.1f, 0.1f, 0));

    //}

    public static Vector2Int GetLogicVectorWithDirectionEnum(Direction _dir)
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

    public MyTile OnMapChange()
    {
        Stack<Vector2Int> _logicalPosStack = new Stack<Vector2Int>();
        Vector2Int _nowLogicalPos = MyTile.ActualToLogicalPosition(_characterObject.transform.position);
        MyTile _trackingTile = _tileDictionary[_nowLogicalPos];
        
        if (_trackingTile.colliding == true)
        {
            _logicalPosStack.Push(_nowLogicalPos);
            bool _found = false;
            while (_logicalPosStack.Count > 0 && _found == false)
            {
                Vector2Int _middlePos = _logicalPosStack.Pop();
                Vector2Int _searchingPos = Vector2Int.zero;
                for (int i = 0; i < DIRECTION_ENUM_LENGTH; i++)
                {
                    _searchingPos = _middlePos + GetLogicVectorWithDirectionEnum((Direction)i);
                    if (_tileDictionary.ContainsKey(_searchingPos))
                    {
                        if(_tileDictionary[_searchingPos].colliding == false)
                        {
                            _found = true;
                            _trackingTile = _tileDictionary[_searchingPos];
                            break;
                        }
                    }
                    _logicalPosStack.Push(_searchingPos);

                }
            }
        }

        _characterObject.transform.position = _trackingTile.actualPos;
        nowStandTile = _trackingTile;
        return nowStandTile;
    }


}


