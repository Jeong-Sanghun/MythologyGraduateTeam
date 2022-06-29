using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileMapGenerator : MonoBehaviour     //class를 만들 땐 MonoBehaviour를 지우는가
{
    private ManagerGroup dataCenter;                          //모두 함수 이름인가
    private TileGenerateDataWrapper _tileGenerateDataWrapper;   //앞에 언더바를 긋는 이유는 무엇인가
    [SerializeField]
    private ItemManager _itemManager;
    [SerializeField]
    private TileMapDataGenerator _tileMapDataGenerator;
    [SerializeField]
    private PlayerControl _playerControl;
    [SerializeField]
    private CreepControl _creepControl;

    public Dictionary<Vector2Int, MyTile> tileDictionary;
    [SerializeField]
    private Tilemap _tileMap;
    [SerializeField]
    private Tile _baseTile;
    [SerializeField]
    private GameObject _colliderTilePrefab;
    [SerializeField]
    private Transform _tileParent;
    [SerializeField]
    private Camera _dumyCamera;
    [SerializeField]
    private GameObject[] _wallPrefabArray;
    [SerializeField]
    float _mapChangeTime;
    private GameObject[] _colliderObjectPool;
    private Vector2[] _colliderObjectPos;
    private int _colliderObjectPoolIndex;

    private bool[,] _nowGeneratedZone;

    public const int ZONE_BOUNDARY = 20;
    bool _tileMapActivationing;
    bool _mapChanging;


    // Start is called before the first frame update
    void Start()
    {
        _tileMapActivationing = false;
        _mapChanging = false;
        dataCenter = ManagerGroup.singleton;     //싱글톤
        dataCenter.tilemapGenerator = this;     //this는 어떻게 쓰이는가
        _nowGeneratedZone = new bool[TileMapDataGenerator.ZONE_X_SIZE,TileMapDataGenerator.ZONE_Y_SIZE];
        SetGeneratedZoneFalse();

        MyTile.TILE_X_SIZE = Constants.TILE_X_SIZE;   //상수 정의 Constants
        MyTile.TILE_Y_SIZE = Constants.TILE_Y_SIZE;
        MyTile.LEFT_DOWN_VECTOR = new Vector2(-MyTile.TILE_X_SIZE, -MyTile.TILE_Y_SIZE);   //x,y값을 리버스
        MyTile.RIGHT_DOWN_VECTOR = new Vector2(MyTile.TILE_X_SIZE, -MyTile.TILE_Y_SIZE);   //y값을 리버스
        MyTile.LEFT_UP_VECTOR = new Vector2(-MyTile.TILE_X_SIZE, MyTile.TILE_Y_SIZE);      //x값을 리버스
        MyTile.RIGHT_UP_VECTOR = new Vector2(MyTile.TILE_X_SIZE, MyTile.TILE_Y_SIZE);      //이건 왜 써야하는가
        MyTile.LEFT_DOWN_VECTOR.Normalize();        //방향벡터 정규화의 이유는 무엇인가
        MyTile.RIGHT_DOWN_VECTOR.Normalize();
        MyTile.LEFT_UP_VECTOR.Normalize();
        MyTile.RIGHT_UP_VECTOR.Normalize();

        SetColliderObjectPool();               //아래에 함수 있음
        _tileGenerateDataWrapper = JsonManager.LoadSaveData<TileGenerateDataWrapper>("TileGenerateDataWrapper");  //이건 뭐지
        _tileGenerateDataWrapper.SetNoneLoadedData();     //이건 뭘까
        StartCoroutine(GenerateTileCoroutineDumy());
        StartCoroutine(CheckPlayerPosAndGenerateCoroutine());
        
    }

    IEnumerator GenerateTileCoroutineDumy()
    {

        bool _onGameStart = true;
        while (true)
        {
            _mapChanging = false;
            yield return new WaitForSeconds(_mapChangeTime);
            _tileMapDataGenerator.GenerateDataAndSave();
            yield return new WaitForSeconds(1f);
            _tileGenerateDataWrapper = JsonManager.LoadSaveData<TileGenerateDataWrapper>("TileGenerateDataWrapper");  //이건 뭐지
            _tileGenerateDataWrapper.SetNoneLoadedData();     //이건 뭘까
            while (_tileMapActivationing == true)
            {
                yield return new WaitForSeconds(0.5f);
            }
            _mapChanging = true;
            StartCoroutine(TileMapActivationCoroutine(_playerControl.GetPlayerLogicalPos(), false));
            yield return new WaitForSeconds(2f);
            while(_tileMapActivationing == true)
            {
                yield return new WaitForSeconds(0.5f);
            }
            
            OnMapChange(_playerControl.GetPlayerLogicalPos());
            _playerControl.OnMapChange();


            for (int i = 0; i < _colliderObjectPool.Length; i++)    //Length는 배열의 길이를 자동으로 계산해준다. 졸라 편함.
            {
                _colliderObjectPool[i].SetActive(false);                     //오브젝트 비활성화
            }

        }
    }

    IEnumerator TileMapActivationCoroutine(Vector2Int _nowLogicalPos,bool _active)
    {
        _tileMapActivationing = true;
        Queue<Vector2Int> _logicalPosQueue = new Queue<Vector2Int>();
        MyTile _trackingTile = tileDictionary[_nowLogicalPos];
        int _waitTime = 20;
        int _nulledTime = 0;
        int _waitCounter = 0;

        _logicalPosQueue.Enqueue(_nowLogicalPos);
        while (_logicalPosQueue.Count > 0)
        {
            _waitCounter++;
            Vector2Int _middlePos = _logicalPosQueue.Dequeue();
            Vector2Int _searchingPos = Vector2Int.zero;
            for (int i = 0; i < ClientPlayerMove.DIRECTION_ENUM_LENGTH; i++)
            {
                _searchingPos = _middlePos + ClientPlayerMove.GetLogicVectorWithDirectionEnum((Direction)i);
                if (tileDictionary.ContainsKey(_searchingPos))
                {
                    if(tileDictionary[_searchingPos].activated == !_active)
                    {

                        TileMapActivation(_searchingPos, _active);
                        _logicalPosQueue.Enqueue(_searchingPos);
                    }
                }
                


            }
            if (_waitCounter < _waitTime * 4)
            {
                yield return null;
            }
            else if (_waitCounter % _waitTime == 0 && _nulledTime < 40) 
            {
                _nulledTime++;
                yield return null;
            }
            
        }
        _tileMapActivationing = false;
    }

    void TileMapActivation(Vector2Int _logicalPos,bool _active)
    {
        TileMapActivation(tileDictionary[_logicalPos], _active);
    }

    void TileMapActivation(MyTile _tile, bool _active)
    {
        if (_active == false)
        {
            _tile.activated = false;
            _tile.tileChangeData.color = Color.black;
            _tileMap.SetTile(_tile.tileChangeData, true);
            _tile.DestroyObject();
        }
        else
        {
            _tile.activated = true;
            _tile.tileChangeData.color = Color.white;
            _tileMap.SetTile(_tile.tileChangeData, true);
            _tile.ActiveObject();
        }
        
    }

    MyTileWrapper LoadTileMapWrapper(Vector2Int _zone)
    {
        return JsonManager.LoadSaveData<MyTileWrapper>("MyTileWrapper" + _zone.x.ToString() + _zone.y.ToString());
    }

    // Update is called once per frame

    void SetColliderObjectPool()   //오브젝트 충돌판정을 의미하는가

    {
        _colliderObjectPoolIndex = 0;
        _colliderObjectPool = new GameObject[5];    //pool이란 정확히 무엇인가
        _colliderObjectPos = new Vector2[5];
        for(int i = 0; i < _colliderObjectPool.Length; i++)    //Length는 배열의 길이를 자동으로 계산해준다. 졸라 편함.
        {
            _colliderObjectPool[i] = Instantiate(_colliderTilePrefab, _tileParent);   //Instantiate는 게임오브젝트를 실행 도중 실시간으로 생성한다.
            _colliderObjectPool[i].SetActive(false);                     //오브젝트 비활성화
            _colliderObjectPos[i] = Vector2.up;                     //위로 한칸
        }
    }

    void GenerateZoneTile(Vector2Int _zoneIndex,bool _onMapChange)    //타일 생성
    {
        List<MyTile> _itemTileList = new List<MyTile>();
        List<MyTile> _wallTileList = new List<MyTile>();
        List<MyTile> _creepTileList = new List<MyTile>();
        if(_nowGeneratedZone[_zoneIndex.x,_zoneIndex.y] ==true){
            return;
        }
        _nowGeneratedZone[_zoneIndex.x,_zoneIndex.y] = true;
        MyTileWrapper _myTileWrapper = LoadTileMapWrapper(_zoneIndex);
        Vector2Int _zoneStartIndex = new Vector2Int(_zoneIndex.x*TileMapDataGenerator.MAP_X_SIZE, _zoneIndex.y * TileMapDataGenerator.MAP_Y_SIZE);

 
        for (int i = 0; i < TileMapDataGenerator.MAP_X_SIZE; i++)
        {
            for (int j = 0; j < TileMapDataGenerator.MAP_Y_SIZE; j++)
            {
                MyTile _tile = _myTileWrapper.tileArray[i*TileMapDataGenerator.MAP_Y_SIZE + j];
                Vector2Int _intPos = _zoneStartIndex+ new Vector2Int(i, j);
                //TileGenerateData _data = null;
                //if(i ==0|| j==0|| i==TileMapDataGenerator.MAP_X_SIZE-1 || 
                //    j== TileMapDataGenerator.MAP_Y_SIZE - 1)
                //{
                //    _data = _tileGenerateDataWrapper.GetDefaultTile();
                //}
                //else
                //{
                //   _data = _tileGenerateDataWrapper.GetRandomTile();
                //}
                //_tile.GenerateTileWithNoData(_data,_intPos, _baseTile);   //GT, Add, ST는 어떤 의미인가
                _tile.GenerateTile(_tileGenerateDataWrapper,_baseTile,_zoneIndex,_onMapChange);
                if (_tile.tileType == TileType.PackedItem)
                {
                    _itemTileList.Add(_tile);
                }
                else if (_tile.tileType == TileType.Wall)
                {
                    _wallTileList.Add(_tile);
                }
                else if (_tile.tileType == TileType.Creep)
                {
                    _creepTileList.Add(_tile);
                }
                tileDictionary.Add(_intPos, _tile);
                Vector3Int _forTileMapPos = new Vector3Int(_intPos.x,_intPos.y);
                _tileMap.SetTileFlags(_forTileMapPos, TileFlags.None);
                _tileMap.SetTile(_tile.tileChangeData, true);

            }
        }



        for (int i = 0; i < _itemTileList.Count; i++)
        {
            _itemManager.SpawnItem(_itemTileList[i]);
        }
        for (int i = 0; i < _wallTileList.Count; i++)
        {
            if (_wallTileList[i].tileType == TileType.Wall)
            {
                _wallTileList[i].tileObject = Instantiate(_wallPrefabArray[(int)_wallTileList[i].tileEnvironment], _tileParent);
                _wallTileList[i].tileObject.transform.position = _wallTileList[i].actualPos;
                if (_onMapChange == true)
                {
                    _wallTileList[i].tileObject.SetActive(false);
                }
                else
                {
                    _wallTileList[i].tileObject.SetActive(true);
                }

            }
        }
        for (int i = 0; i < _creepTileList.Count; i++)
        {
            if (_creepTileList[i].tileType == TileType.Creep)
            {
                _creepControl.SpawnCreep(_creepTileList[i], _onMapChange);

            }
        }



        //다만들어주면 레이드.
    }

    public void OnMapChange(Vector2Int _playerLogicalPos)
    {
        if(tileDictionary != null)
        {
            tileDictionary.Clear();
            _tileMap.ClearAllTiles();
            _itemManager.OnMapChange();
            
        }
        else
        {
            tileDictionary = new Dictionary<Vector2Int, MyTile>();    //동적할당
            _itemManager.tileDictionary = tileDictionary;   //진짜실타
        }
        _itemManager.OnMapChange();
        SetGeneratedZoneFalse();
        CheckPlayerAndGenerateZone(_playerLogicalPos,true);
        StartCoroutine(TileMapActivationCoroutine(_playerControl.GetPlayerLogicalPos(), true));
    }

    IEnumerator CheckPlayerPosAndGenerateCoroutine()
    {
        WaitForSeconds _second = new WaitForSeconds(1f);
        while (true)
        {
            yield return _second;
            if(_tileMapActivationing == false && _mapChanging == false)
            {
                CheckPlayerAndGenerateZone(_playerControl.GetPlayerLogicalPos(), false);
            }
            
        }
    }

    public void CheckPlayerAndGenerateZone(Vector2Int _playerLogicalPos,bool _onMapChange)
    {
        for(int i = 0; i < TileMapDataGenerator.ZONE_X_SIZE; i++)
        {
            for (int j = 0; j < TileMapDataGenerator.ZONE_Y_SIZE; j++)
            {
                if (_nowGeneratedZone[i, j] == true)
                {
                    continue;
                }
                Vector2Int _leftDownPos = new Vector2Int(i * TileMapDataGenerator.MAP_X_SIZE, j * TileMapDataGenerator.MAP_Y_SIZE);
                Vector2Int _rightUpPos = new Vector2Int((i + 1) * TileMapDataGenerator.MAP_X_SIZE, (j + 1) * TileMapDataGenerator.MAP_Y_SIZE);
                if (_playerLogicalPos.x < _leftDownPos.x - ZONE_BOUNDARY || _playerLogicalPos.x > _rightUpPos.x + ZONE_BOUNDARY)
                {
                    continue;
                }
                if (_playerLogicalPos.y < _leftDownPos.y - ZONE_BOUNDARY || _playerLogicalPos.y > _rightUpPos.y + ZONE_BOUNDARY)
                {
                    continue;
                }
                bool _inBound = false;
                if (_playerLogicalPos.x > _leftDownPos.x - ZONE_BOUNDARY && _playerLogicalPos.x < _leftDownPos.x + ZONE_BOUNDARY)
                {
                    _inBound = true;
                }
                else if (_playerLogicalPos.y > _leftDownPos.y - ZONE_BOUNDARY && _playerLogicalPos.y < _leftDownPos.y + ZONE_BOUNDARY)
                {
                    _inBound = true;
                }
                else if (_playerLogicalPos.x > _rightUpPos.x - ZONE_BOUNDARY && _playerLogicalPos.x < _rightUpPos.x + ZONE_BOUNDARY)
                {
                    _inBound = true;
                }
                else if (_playerLogicalPos.y > _rightUpPos.y - ZONE_BOUNDARY && _playerLogicalPos.y < _rightUpPos.y + ZONE_BOUNDARY)
                {
                    _inBound = true;
                }
                
                if(_inBound == false)
                {
                    continue;
                }
                GenerateZoneTile(new Vector2Int(i,j),_onMapChange);
            }
        }


    }
    
    

    public void SetColliderObject(MyTile _tile)   //어디에 쓰는 함수일까
    {
        if(_tile.colliding == false)    //충돌이 없으면 정지인가
        {
            return;
        }
        for(int i = 0; i < _colliderObjectPos.Length; i++)
        {
            if(_colliderObjectPos[i] == _tile.actualPos)    //이건 무슨 의미일까, _colliderObjectPos를 tile.actualPos와 같아질 때까지 반복한다
            {
                return;
            }
        }
        _colliderObjectPoolIndex++;      //_colliderObjectPoolIndex 1 증가
        if (_colliderObjectPoolIndex >= _colliderObjectPool.Length)  //_colliderObjectPoolIndex가 _colliderObjectPool 배열의 길이보다 크거나 같으면 0으로 한다
        {
            _colliderObjectPoolIndex = 0;
        }
        GameObject _col_obj = _colliderObjectPool[_colliderObjectPoolIndex];   //_col_obj에 b를 대입
        _colliderObjectPos[_colliderObjectPoolIndex] = _tile.actualPos;       //b에 c를 대입
        //_tile.tileObject = _col_obj;                                          //d에 _col_obj를 대입
        _col_obj.transform.position = _tile.actualPos;                        //_col_obj의 포지션에 c대입 
        _col_obj.SetActive(true);                                             //이하의 과정을 하는 이유가 뭘까
    }

    public void SetActiveFalseCollider()
    {
        for(int i = 0; i < _colliderObjectPool.Length; i++)
        {
            _colliderObjectPool[i].SetActive(false);
        }
    }

    void SetGeneratedZoneFalse()
    {
        for (int i = 0; i < TileMapDataGenerator.ZONE_X_SIZE; i++)
        {
            for (int j = 0; j < TileMapDataGenerator.ZONE_Y_SIZE; j++)
            {
                _nowGeneratedZone[i, j] = false;
            }
        }
    }

    //private void OnDrawGizmos()
    //{
    //    for(int i = 0; i < _gizmoList.Count-1; i++)
    //    {
    //        Vector2 _a = _gizmoList[i];
    //        Vector2 _b = _gizmoList[i + 1];
    //        Gizmos.DrawLine(_a, _b);
    //    }
    //}
}
