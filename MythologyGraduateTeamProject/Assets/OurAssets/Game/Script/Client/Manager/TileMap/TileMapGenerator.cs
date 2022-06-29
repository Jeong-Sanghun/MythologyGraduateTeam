using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileMapGenerator : MonoBehaviour     //class�� ���� �� MonoBehaviour�� ����°�
{
    private ManagerGroup dataCenter;                          //��� �Լ� �̸��ΰ�
    private TileGenerateDataWrapper _tileGenerateDataWrapper;   //�տ� ����ٸ� �ߴ� ������ �����ΰ�
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
        dataCenter = ManagerGroup.singleton;     //�̱���
        dataCenter.tilemapGenerator = this;     //this�� ��� ���̴°�
        _nowGeneratedZone = new bool[TileMapDataGenerator.ZONE_X_SIZE,TileMapDataGenerator.ZONE_Y_SIZE];
        SetGeneratedZoneFalse();

        MyTile.TILE_X_SIZE = Constants.TILE_X_SIZE;   //��� ���� Constants
        MyTile.TILE_Y_SIZE = Constants.TILE_Y_SIZE;
        MyTile.LEFT_DOWN_VECTOR = new Vector2(-MyTile.TILE_X_SIZE, -MyTile.TILE_Y_SIZE);   //x,y���� ������
        MyTile.RIGHT_DOWN_VECTOR = new Vector2(MyTile.TILE_X_SIZE, -MyTile.TILE_Y_SIZE);   //y���� ������
        MyTile.LEFT_UP_VECTOR = new Vector2(-MyTile.TILE_X_SIZE, MyTile.TILE_Y_SIZE);      //x���� ������
        MyTile.RIGHT_UP_VECTOR = new Vector2(MyTile.TILE_X_SIZE, MyTile.TILE_Y_SIZE);      //�̰� �� ����ϴ°�
        MyTile.LEFT_DOWN_VECTOR.Normalize();        //���⺤�� ����ȭ�� ������ �����ΰ�
        MyTile.RIGHT_DOWN_VECTOR.Normalize();
        MyTile.LEFT_UP_VECTOR.Normalize();
        MyTile.RIGHT_UP_VECTOR.Normalize();

        SetColliderObjectPool();               //�Ʒ��� �Լ� ����
        _tileGenerateDataWrapper = JsonManager.LoadSaveData<TileGenerateDataWrapper>("TileGenerateDataWrapper");  //�̰� ����
        _tileGenerateDataWrapper.SetNoneLoadedData();     //�̰� ����
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
            _tileGenerateDataWrapper = JsonManager.LoadSaveData<TileGenerateDataWrapper>("TileGenerateDataWrapper");  //�̰� ����
            _tileGenerateDataWrapper.SetNoneLoadedData();     //�̰� ����
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


            for (int i = 0; i < _colliderObjectPool.Length; i++)    //Length�� �迭�� ���̸� �ڵ����� ������ش�. ���� ����.
            {
                _colliderObjectPool[i].SetActive(false);                     //������Ʈ ��Ȱ��ȭ
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

    void SetColliderObjectPool()   //������Ʈ �浹������ �ǹ��ϴ°�

    {
        _colliderObjectPoolIndex = 0;
        _colliderObjectPool = new GameObject[5];    //pool�̶� ��Ȯ�� �����ΰ�
        _colliderObjectPos = new Vector2[5];
        for(int i = 0; i < _colliderObjectPool.Length; i++)    //Length�� �迭�� ���̸� �ڵ����� ������ش�. ���� ����.
        {
            _colliderObjectPool[i] = Instantiate(_colliderTilePrefab, _tileParent);   //Instantiate�� ���ӿ�����Ʈ�� ���� ���� �ǽð����� �����Ѵ�.
            _colliderObjectPool[i].SetActive(false);                     //������Ʈ ��Ȱ��ȭ
            _colliderObjectPos[i] = Vector2.up;                     //���� ��ĭ
        }
    }

    void GenerateZoneTile(Vector2Int _zoneIndex,bool _onMapChange)    //Ÿ�� ����
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
                //_tile.GenerateTileWithNoData(_data,_intPos, _baseTile);   //GT, Add, ST�� � �ǹ��ΰ�
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



        //�ٸ�����ָ� ���̵�.
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
            tileDictionary = new Dictionary<Vector2Int, MyTile>();    //�����Ҵ�
            _itemManager.tileDictionary = tileDictionary;   //��¥��Ÿ
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
    
    

    public void SetColliderObject(MyTile _tile)   //��� ���� �Լ��ϱ�
    {
        if(_tile.colliding == false)    //�浹�� ������ �����ΰ�
        {
            return;
        }
        for(int i = 0; i < _colliderObjectPos.Length; i++)
        {
            if(_colliderObjectPos[i] == _tile.actualPos)    //�̰� ���� �ǹ��ϱ�, _colliderObjectPos�� tile.actualPos�� ������ ������ �ݺ��Ѵ�
            {
                return;
            }
        }
        _colliderObjectPoolIndex++;      //_colliderObjectPoolIndex 1 ����
        if (_colliderObjectPoolIndex >= _colliderObjectPool.Length)  //_colliderObjectPoolIndex�� _colliderObjectPool �迭�� ���̺��� ũ�ų� ������ 0���� �Ѵ�
        {
            _colliderObjectPoolIndex = 0;
        }
        GameObject _col_obj = _colliderObjectPool[_colliderObjectPoolIndex];   //_col_obj�� b�� ����
        _colliderObjectPos[_colliderObjectPoolIndex] = _tile.actualPos;       //b�� c�� ����
        //_tile.tileObject = _col_obj;                                          //d�� _col_obj�� ����
        _col_obj.transform.position = _tile.actualPos;                        //_col_obj�� �����ǿ� c���� 
        _col_obj.SetActive(true);                                             //������ ������ �ϴ� ������ ����
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
