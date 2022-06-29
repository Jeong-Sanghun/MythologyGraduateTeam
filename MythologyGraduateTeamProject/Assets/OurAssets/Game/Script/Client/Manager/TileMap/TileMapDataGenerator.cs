using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileMapDataGenerator : MonoBehaviour
{
    private TileGenerateDataWrapper _tileGenerateDataWrapper;   //앞에 언더바를 긋는 이유는 무엇인가
    public const int MAP_X_SIZE = 30;
    public const int MAP_Y_SIZE = 30;
    public const int ZONE_X_SIZE = 4;
    public const int ZONE_Y_SIZE = 4;
    public const int ENVIRONMENT_RADIUS = 2;
    MyTile _convexHullStartTile;
    int _environmentLength;

    // Start is called before the first frame update
    void Start()
    {
        _tileGenerateDataWrapper = JsonManager.LoadSaveData<TileGenerateDataWrapper>("TileGenerateDataWrapper");  //이건 뭐지
        _tileGenerateDataWrapper.SetNoneLoadedData();     //이건 뭘까
        GenerateDataAndSave();
       

    }

    public void GenerateDataAndSave()
    {
        _environmentLength = System.Enum.GetValues(typeof(TileEnvironment)).Length;
        MyTileWrapper _tileWrapper = new MyTileWrapper();
        MyTile[] _tileArray = new MyTile[MAP_X_SIZE*MAP_Y_SIZE];
        for (int i = 0; i < MAP_X_SIZE; i++)
        {
            for (int j = 0; j < MAP_Y_SIZE; j++)
            {
                _tileArray[i * MAP_Y_SIZE + j] = new MyTile();
            }
        }
        for (int x = 0; x < ZONE_X_SIZE; x++)
        {
            for (int y = 0; y < ZONE_Y_SIZE; y++)
            {
                List<MyTile> _connectingWallTileList = new List<MyTile>();
                List<MyTile> _wholeWallTileList = new List<MyTile>();
                for (int i = 0; i < MAP_X_SIZE; i++)
                {
                    for (int j = 0; j < MAP_Y_SIZE; j++)
                    {
                        _tileArray[i * MAP_Y_SIZE + j] = new MyTile();
                        TileGenerateData _data = _tileGenerateDataWrapper.GetRandomTile();  //타일 랜덤 생성인가
                        if(_data.tileType == TileType.Wall)
                        {
                            _wholeWallTileList.Add(_tileArray[i * MAP_Y_SIZE + j]);
                        }
                        int _plussedIndex = x * MAP_X_SIZE + i + y * MAP_Y_SIZE + j;
                        int _modular = _plussedIndex % (MAP_X_SIZE * _environmentLength);
                        float _environmentNumber = (float)_modular / MAP_X_SIZE;
                        int _environmentIndex = (int)_environmentNumber % _environmentLength;
                        int _firstMinority = (int)(_environmentNumber * 10) % 10;
                        int _upOrDown = 0;
                        if (_firstMinority > 5)
                        {
                            if (_firstMinority >= 10- ENVIRONMENT_RADIUS && Random.Range(0,3) == 0)
                            {
                                _upOrDown = 1;
                            }
                            else
                            {
                                _upOrDown = 0;
                            }
                        }
                        else
                        {
                            if (_firstMinority < ENVIRONMENT_RADIUS && Random.Range(0, 3) == 0)
                            {
                                _upOrDown = -1;
                            }
                            else
                            {
                                _upOrDown = 0;
                            }
                        }

                   

                        
                        _environmentIndex += _upOrDown + _environmentLength;
                        
                        _environmentIndex %= _environmentLength;
                        
                        _tileArray[i * MAP_Y_SIZE + j].GenerateData(new Vector2Int(i, j), _data.tileType,(TileEnvironment)_environmentIndex);
                    }
                }

                //이제 벽돌들 이어주는 작업.
                //여기서 컨벡스홀을 돌린다.


                if (_wholeWallTileList.Count != 0)//&& Random.Range(0,1)==0)
                {
                    ConvexHull(_wholeWallTileList, _connectingWallTileList);

                    MyTile _startWallTile = _connectingWallTileList[0];
                    MyTile _nowWallTile = _connectingWallTileList[0];
                    MyTile _nextWallTile = null;
                    _connectingWallTileList.RemoveAt(0);
                    while (true)
                    {
                        bool _startToEndGraphed = false;
                        Vector2Int _startWallPos = _nowWallTile.logicalPos;
                        Vector2Int _endWallPos = Vector2Int.zero;
                        Vector2Int _startToEndGap = Vector2Int.zero;
                        float _xGap = 1;
                        float _yGap = 1;
                        if (_connectingWallTileList.Count != 0)
                        {

                            _nextWallTile = _connectingWallTileList[0];

                        }

                        if (_nextWallTile == null)
                        {
                            _nextWallTile = _startWallTile;
                            _startToEndGraphed = true;
                        }

                        if (_nextWallTile == _nowWallTile)
                        {
                            break;
                        }
                        _endWallPos = _nextWallTile.logicalPos;
                        _startToEndGap = _endWallPos - _startWallPos;


                        //if (Mathf.Abs(_startToEndGap.x) > Mathf.Abs(_startToEndGap.y))
                        //{
                        //    _xGap = ((float)Mathf.Abs(_startToEndGap.x)) / _startToEndGap.x;
                        //    _yGap = (float)_startToEndGap.y / _startToEndGap.x;

                        //}
                        //else if (Mathf.Abs(_startToEndGap.x) < Mathf.Abs(_startToEndGap.y))
                        //{
                        //    _xGap = (float)_startToEndGap.x / _startToEndGap.y;
                        //    _yGap = ((float)Mathf.Abs(_startToEndGap.y)) / _startToEndGap.y;

                        //}
                        //else if(_startToEndGap.x != 0)
                        //{
                        //    _xGap = ((float)Mathf.Abs(_startToEndGap.x)) / _startToEndGap.x;
                        //    _yGap = ((float)Mathf.Abs(_startToEndGap.y)) / _startToEndGap.y;
                        //}
                        Vector2 _gap = _startToEndGap;
                        _gap.Normalize();
                        if (Mathf.Abs(_gap.x) > Mathf.Abs(_gap.y))
                        {
                            _gap.y *= 1 / Mathf.Abs(_gap.x);
                            _gap.x *= 1 / Mathf.Abs(_gap.x);
                        }
                        else
                        {
                            _gap.x *= 1 / Mathf.Abs(_gap.y);
                            _gap.y *= 1 / Mathf.Abs(_gap.y);
                        }
                        

                        float _xTracker = 0;
                        float _yTracker = 0;
                        int _counter = 0;
                        while (Mathf.Abs(_xTracker) <= Mathf.Abs(_startToEndGap.x) || Mathf.Abs(_yTracker) <= Mathf.Abs(_startToEndGap.y))
                        {
                            _counter++;
                            if (_counter > 1000)
                            {
                                Debug.Log("뭐여");
                                break;
                            }
                            _xTracker += _gap.x;
                            _yTracker += _gap.y;
                            int _index = (_startWallPos.x + (int)_xTracker) * MAP_Y_SIZE + _startWallPos.y + (int)_yTracker;
                            if (_index >= _tileArray.Length || _index < 0)
                            {
                                continue;
                            }
                            MyTile _tile = _tileArray[_index];
                            //if(_tile.tileType == TileType.Wall)
                            //{
                            //    break;
                            //}
                            if (_tile == _nextWallTile)
                            {
                                
                                break;
                            }

                            _tile.tileType = TileType.Wall;
                          



                        }
                        _nowWallTile = _nextWallTile;
                        _nextWallTile = null;
                        if (_connectingWallTileList.Count != 0)
                        {
                            _connectingWallTileList.RemoveAt(0);

           

                        }
                        if (_startToEndGraphed == true)
                        {
                            break;
                        }
                    }
                }

                _tileWrapper.tileArray = _tileArray;
                
                JsonManager.SaveJson<MyTileWrapper>(_tileWrapper,"MyTileWrapper" + x.ToString() + y.ToString());
            }
        }
    }

    void ConvexHull(List<MyTile> _tileList,List<MyTile> _outputTileList)
    {
        //x가 제일 작은 타일 골라내기
        List<MyTile> _wholeTileList = new List<MyTile>();
        for (int i =0; i < _tileList.Count; i++)
        {
            _wholeTileList.Add(_tileList[i]);
        }

        Stack<MyTile> _tileStack = new Stack<MyTile>();
        _convexHullStartTile = _wholeTileList[0];
        for (int i = 1; i < _wholeTileList.Count; i++)
        {
            if (_convexHullStartTile.logicalPos.x< _wholeTileList[i].logicalPos.x)
            {
                _convexHullStartTile = _wholeTileList[i];
            }
        }

        _wholeTileList.Remove(_convexHullStartTile);
        _tileStack.Push(_convexHullStartTile);
        //Debug.Log("스타트 " + _convexHullStartTile.logicalPos);

        _wholeTileList.Sort(ConvexHullComparer);
        _tileStack.Push(_wholeTileList[0]);
        //Debug.Log("첫타일 " + _wholeTileList[0].logicalPos);

        //for (int i = 0; i < _wholeTileList.Count; i++)
        //{
        //    Debug.Log("체크 " + i + " 인덱스" + CheckAngle(_convexHullStartTile.logicalPos, _convexHullStartTile.logicalPos + Vector2Int.right, _wholeTileList[i].logicalPos));
        //}

        int _trackingIndex = 1;
        while (_trackingIndex<_wholeTileList.Count)
        {
           
            while (_tileStack.Count >= 2)
            {

                MyTile _endTile = _tileStack.Pop();
                MyTile _startTile = _tileStack.Peek();
                MyTile _trackingTile = _wholeTileList[_trackingIndex];
                float _angle = CheckAngle(_startTile.logicalPos, _endTile.logicalPos, _trackingTile.logicalPos);
                if (_angle <180)
                {
                    _tileStack.Push(_endTile);
                    break;
                }

            }
            _tileStack.Push(_wholeTileList[_trackingIndex]);
            _trackingIndex++;
            
        }

        while (_tileStack.Count > 0)
        {
            _outputTileList.Add(_tileStack.Pop());
        }
        
    }

    int ConvexHullComparer(MyTile _aTile, MyTile _bTile)
    {
        Vector2Int _aPos = _aTile.logicalPos;
        Vector2Int _bPos = _bTile.logicalPos;
        Vector2Int _startPos = _convexHullStartTile.logicalPos;
        float _aAngle = CheckAngle(_startPos, _startPos+ Vector2Int.right , _aPos);
        float _bAngle = CheckAngle(_startPos, _startPos+ Vector2Int.right, _bPos);
        if (_aAngle > _bAngle)
        {
            return 1;
        }
        else if (_aAngle == _bAngle)
        {
            return 0;
        }
        else
        {
            return -1;
        }
    }

    float CheckAngle(Vector2Int _logicStart, Vector2Int _logicEnd, Vector2Int _logicChecker)
    {
        Vector2 _start = _logicStart;
        Vector2 _end = _logicEnd;
        Vector2 _checker = _logicChecker;

        Vector2 _originDirection = _end - _start;
        Vector2 _checkingDirection = _checker - _start;
        //_originDirection.Normalize();
        //_checkingDirection.Normalize();
        //float _dottedValue= Vector2.SignedAngle(_originDirection, _checkingDirection);
        //float _angle = Mathf.Acos(_dottedValue);
        float _angle = Vector2.SignedAngle(_originDirection, _checkingDirection);
        if (_angle < 0)
        {
            _angle += 360;
        }
       // Debug.Log("오리진 " + _originDirection + " 체크 " + _checkingDirection + " 앵글 " + _angle);
        return _angle;
    }


}

//벽 이어주는 코드

//if (Mathf.Abs(_startToEndGap.x) > Mathf.Abs(_startToEndGap.y))
//{
//    _xGap = ((float)Mathf.Abs(_startToEndGap.x)) / _startToEndGap.x;
//    _yGap = (float)_startToEndGap.y / _startToEndGap.x;

//}
//else if (Mathf.Abs(_startToEndGap.x) < Mathf.Abs(_startToEndGap.y))
//{
//    _xGap = (float)_startToEndGap.x / _startToEndGap.y;
//    _yGap = ((float)Mathf.Abs(_startToEndGap.y)) / _startToEndGap.y;

//}
//else if (_startToEndGap.x != 0)
//{
//    _xGap = ((float)Mathf.Abs(_startToEndGap.x)) / _startToEndGap.x;
//    _yGap = ((float)Mathf.Abs(_startToEndGap.y)) / _startToEndGap.y;
//}

//float _xTracker = 0;
//float _yTracker = 0;
//int _counter = 0;
////while (Mathf.Abs(_xTracker) <= Mathf.Abs(_startToEndGap.x) || Mathf.Abs(_yTracker) <= Mathf.Abs(_startToEndGap.y))
//while (true)
//{
//    _counter++;
//    if (_counter > 1000)
//    {
//        Debug.Log("뭐여");
//        break;
//    }
//    _xTracker += _xGap;
//    _yTracker += _yGap;
//    int _index = (_startWallPos.x + (int)_xTracker) * MAP_Y_SIZE + _startWallPos.y + (int)_yTracker;
//    if (_index >= _tileArray.Length || _index < 0)
//    {
//        break;
//    }
//    MyTile _tile = _tileArray[_index];
//    //if(_tile.tileType == TileType.Wall)
//    //{
//    //    break;
//    //}
//    if (_tile == _nextWallTile)
//    {

//        break;
//    }

//    _tile.tileType = TileType.Wall;
//    _tile.tileEnvironment = (TileEnvironment)(((int)_tile.tileEnvironment + 1) % _environmentLength);



//}


//대격변 코드 이전거. 최단거리 대격변.

//if (_wallTileList.Count != 0 )//&& Random.Range(0,1)==0)
//{
//    MyTile _startWallTile = _wallTileList[0];
//    MyTile _nowWallTile = _wallTileList[0];
//    MyTile _nextWallTile = null;
//    _wallTileList.RemoveAt(0);
//    while (true)
//    {
//        bool _startToEndGraphed = false;
//        Vector2Int _startWallPos = _nowWallTile.logicalPos;
//        Vector2Int _endWallPos = Vector2Int.zero;
//        Vector2Int _startToEndGap = Vector2Int.zero;
//        float _xGap = 0;
//        float _yGap = 0;

//        float _minMagnitude = MAP_X_SIZE * MAP_Y_SIZE;
//        _minMagnitude *= _minMagnitude;
//        for (int i = 0; i < _wallTileList.Count; i++)
//        {
//            float _checkingMagnitude = (_startWallPos - _wallTileList[i].logicalPos).sqrMagnitude;
//            if (_checkingMagnitude < _minMagnitude)
//            {
//                _minMagnitude = _checkingMagnitude;
//                _nextWallTile = _wallTileList[i];
//            }
//        }

//        if (_nextWallTile == null)
//        {
//            break;
//        }

//        if (_nextWallTile == _startWallTile)
//        {
//            _startToEndGraphed = true;
//        }
//        if(_nextWallTile == _nowWallTile)
//        {
//            break;
//        }
//        _endWallPos = _nextWallTile.logicalPos;
//        _startToEndGap = _endWallPos - _startWallPos;


//        if (Mathf.Abs(_startToEndGap.x) > Mathf.Abs(_startToEndGap.y))
//        {
//            _xGap = (float)Mathf.Abs(_startToEndGap.x) / _startToEndGap.x; 
//            _yGap = (float)_startToEndGap.y / _startToEndGap.x;

//        }
//        else
//        {
//            _xGap = (float)_startToEndGap.x / _startToEndGap.y;
//            _yGap = (float)Mathf.Abs(_startToEndGap.y) / _startToEndGap.y;

//        }

//        float _xTracker = 0;
//        float _yTracker = 0;
//        int _counter = 0;
//        while (Mathf.Abs(_xTracker) <= Mathf.Abs(_startToEndGap.x) || Mathf.Abs(_yTracker) <= Mathf.Abs(_startToEndGap.y))
//        {
//            _counter++;
//            if (_counter > 1000)
//            {
//                Debug.Log("뭐여");
//                break;
//            }
//            _xTracker += _xGap;
//            _yTracker += _yGap;
//            int _index = (_startWallPos.y + (int)_yTracker) * MAP_Y_SIZE + _startWallPos.x + (int)_xTracker;
//            if(_index >= _tileArray.Length || _index < 0)
//            {
//                continue;
//            }
//            MyTile _tile = _tileArray[_index];
//            if (_tile == _nextWallTile)
//            {
//                Debug.Log("도착!");
//                break;
//            }

//            _tile.tileType = TileType.Wall;



//        }





//        _nowWallTile = _nextWallTile;
//        _nextWallTile = null;
//        if(_wallTileList.Count != 0)
//        {
//            _wallTileList.Remove(_nowWallTile);
//            if (_wallTileList.Count == 0)
//            {
//                _nextWallTile = _startWallTile;
//            }

//        }

//        if(_startToEndGraphed == true)
//        {
//            break;
//        }
//    }
//}