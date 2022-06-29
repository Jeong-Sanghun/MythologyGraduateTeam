using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Tilemaps;

public enum CollidingSide
{
    None,LeftUp,RightUp,LeftDown,RightDown,Left,Right,Up,Down
}



[Serializable]
public class MyTile
{
    [NonSerialized]
    public static float TILE_X_SIZE;
    [NonSerialized]
    public static float TILE_Y_SIZE;
    [NonSerialized]
    public static Vector2 LEFT_UP_VECTOR;
    [NonSerialized]
    public static Vector2 LEFT_DOWN_VECTOR;
    [NonSerialized]
    public static Vector2 RIGHT_UP_VECTOR;
    [NonSerialized]
    public static Vector2 RIGHT_DOWN_VECTOR;
    [NonSerialized]
    public TileGenerateData tileData;
    public Vector2Int logicalPos;
    public TileEnvironment tileEnvironment;
    public TileType tileType;

    [NonSerialized]
    public bool interactioned;

    [NonSerialized]
    public bool colliding;
    [NonSerialized]
    public GameObject tileObject;
    [NonSerialized]
    public Vector2 actualPos;
    [NonSerialized]
    public bool isGenerated;
    [NonSerialized]
    public static int tileTypeLength;
    [NonSerialized]
    public TileChangeData tileChangeData;
    [NonSerialized]
    public List<MyTile> raidChildTileList; //보스타일에서만 쓰도록 하자.
    [NonSerialized]
    public MyTile raidParentTIle;

    [NonSerialized]
    public PackedItem nowPackedItem;
    [NonSerialized]
    public DroppedItem nowDroppedItem;
    [NonSerialized]
    public ItemData itemData;
    [NonSerialized]
    public bool activated;
    [NonSerialized]
    public Creep creep;
    static Transform _itemParent;
    static ItemManager _itemManager;

    public MyTile()
    {
        logicalPos = new Vector2Int();
        tileType = TileType.Default;
        interactioned = false;
        colliding = false;
        tileObject = null;
        isGenerated = false;
        activated = false;
        creep = null;

        if (tileTypeLength == 0)
        {
            tileTypeLength = Enum.GetValues(typeof(TileType)).Length;
        }

    }

    public static void SetItemManager(ItemManager _parameter,Transform _parent)
    {
        
        if (_itemManager == null)
        {
            _itemParent = _parent;
            _itemManager = _parameter;
        }
    }

    //랜덤값도 정해줌
    //public void SetValue(Vector2Int _pos, GameObject _tile_prefab, Transform _tile_parent)
    //{
    //    logicalPos = _pos;
    //    tileType = (TileType)UnityEngine.Random.Range(0, tileTypeLength);
    //    switch (tileType)
    //    {
    //        case TileType.Gas:
    //        case TileType.Mineral:
    //        case TileType.Rock:
    //            colliding = true;
    //            break;
    //        default:
    //            colliding = false;
    //            break;

    //    }

    //    tileObject = GameObject.Instantiate(_tile_prefab, _tile_parent);
    //    //if(_pos.y % 2 == 0)
    //    //{
    //    //    actual_pos = new Vector2(_pos.x * tile_x_size, (_pos.x+_pos.y) * tile_y_size);

    //    //}
    //    //else
    //    //{
    //    //    actual_pos = new Vector2(_pos.x * tile_x_size + 0.5f * tile_x_size, _pos.y * tile_y_size);
    //    //}
    //    actualPos = new Vector2((_pos.x - _pos.y) * TILE_X_SIZE * 0.5f, (_pos.x + _pos.y) * TILE_Y_SIZE * 0.5f);
    //    tileObject.transform.position = actualPos;


    //}

    public void GenerateData(Vector2Int _pos, TileType _type,TileEnvironment _environment)
    {
        logicalPos = _pos;
        tileType = _type;
        tileEnvironment = _environment;
    }

    //public void GenerateTileWithNoData(TileGenerateData _data, Vector2Int _pos, Tile _baseTile)
    //{
    //    logicalPos = _pos;
    //    tileObject = null;
    //    tileType = _data.tileType;
    //    if (_pos.x == 0 && _pos.y == 0)
    //    {
    //        tileType = TileType.Default;
    //    }
    //    _baseTile.sprite = _data.tileSprite;


    //    actualPos = new Vector2((_pos.x - _pos.y) * TILE_X_SIZE * 0.5f, (_pos.x + _pos.y) * TILE_Y_SIZE * 0.5f);
    //    switch (tileType)
    //    {
    //        case TileType.Wall:
    //            colliding = true;
    //            break;
    //        case TileType.ClothBox:
    //        case TileType.WeaponBox:
    //            colliding = true;
    //            _itemManager.SpawnItem(this);
    //            break;
    //        default:
    //            colliding = false;
    //            break;

    //    }




    //    Vector3Int _int_pos = new Vector3Int((int)_pos.x, (int)_pos.y);
    //    _tileChangeData = new TileChangeData(_int_pos, _baseTile, Color.white, Matrix4x4.identity);

    //}

    public void GenerateTile(TileGenerateDataWrapper _dataWrapper, Tile _baseTile, Vector2Int _zoneIndex,bool _onMapChange)
    {

        TileGenerateData _data = null;
        tileObject = null;
        for (int i = 0; i < _dataWrapper.tileGenerateDataArray.Length; i++)
        {
            if (_dataWrapper.tileGenerateDataArray[i].tileType == tileType)
            {
               
                _data = _dataWrapper.tileGenerateDataArray[i];
                tileData = _data;
                break;
            }
        }
        if (_data == null)
        {
            Debug.Log("널이여");
            return;
        }
        _baseTile.sprite = _data.tileSpriteArray[(int)tileEnvironment];
        logicalPos += new Vector2Int(_zoneIndex.x * TileMapDataGenerator.MAP_X_SIZE, _zoneIndex.y * TileMapDataGenerator.MAP_Y_SIZE);

        actualPos = new Vector2((logicalPos.x - logicalPos.y) * TILE_X_SIZE * 0.5f, (logicalPos.x + logicalPos.y) * TILE_Y_SIZE * 0.5f);
        switch (tileType)
        {
            case TileType.Wall:
            case TileType.Creep:
            case TileType.PackedItem:
                colliding = true;
                //이거 out으로 넘겨서 리스트에 넣어두고 Spawn함.
                break;
            default:
                colliding = false;
                break;

        }




        Vector3Int _int_pos = new Vector3Int((int)logicalPos.x, (int)logicalPos.y);
        if(_onMapChange == true)
        {
            activated = false;
            tileChangeData = new TileChangeData(_int_pos, _baseTile, Color.black, Matrix4x4.identity);
        }
        else
        {
            activated = true;
            tileChangeData = new TileChangeData(_int_pos, _baseTile, Color.white, Matrix4x4.identity);
        }


    }


    public static Vector2Int ActualToLogicalPosition(Vector2 _actual_pos)
    {


        int _portionX = (int)(_actual_pos.x / (TILE_X_SIZE / 2f));
        int _portionY = (int)(_actual_pos.y / (TILE_Y_SIZE / 2f));
        float _elseX = _actual_pos.x % (TILE_X_SIZE / 2f);
        float _elseY = _actual_pos.y % (TILE_Y_SIZE / 2f);


        if (_elseX < 0)
        {
            _portionX--;
            _elseX += TILE_X_SIZE / 2f;
        }
        if (_elseY < 0)
        {
            _portionY--;
            _elseY += TILE_Y_SIZE / 2f;
        }



        float _portionElseX = _portionX % 2f;
        float _portionElseY = _portionY % 2f;
        if (_portionElseX < 0)
        {
            _portionElseX *= -1;
        }
        if (_portionElseY < 0)
        {
            _portionElseY *= -1;
        }


        Vector2 _normalizedActualPos = new Vector2();
        //왼쪽아래가문제.
        if (_portionElseX != 0)
        {
            if (_portionElseY != 0)
            {
                if (_elseY < -(TILE_Y_SIZE / TILE_X_SIZE) * _elseX + TILE_Y_SIZE / 2f)
                {
                    _normalizedActualPos.x = _portionX;
                    _normalizedActualPos.y = _portionY;

                }
                else
                {
                    // Debug.Log("1,0 위");
                    _normalizedActualPos.x = _portionX + 1;
                    _normalizedActualPos.y = _portionY + 1;
                }
            }
            else
            {

                if (_elseY < _elseX * TILE_Y_SIZE / TILE_X_SIZE)
                {
                    //Debug.Log("1,1 아래");
                    _normalizedActualPos.x = _portionX + 1;
                    _normalizedActualPos.y = _portionY;

                }
                else
                {
                    // Debug.Log("1,1 위");
                    _normalizedActualPos.x = _portionX;
                    _normalizedActualPos.y = _portionY + 1;
                }
            }
        }
        else
        {

            if (_portionElseY != 0)
            {

                if (_elseY < _elseX * TILE_Y_SIZE / TILE_X_SIZE)
                {
                    // Debug.Log("0,0 아래");
                    _normalizedActualPos.x = _portionX + 1;
                    _normalizedActualPos.y = _portionY;

                }
                else
                {
                    // Debug.Log("0,0 위");
                    _normalizedActualPos.x = _portionX;
                    _normalizedActualPos.y = _portionY + 1;
                }


            }
            else
            {

                if (_elseY < -(TILE_Y_SIZE / TILE_X_SIZE) * _elseX + TILE_Y_SIZE / 2f)
                {
                    //Debug.Log("0,1 아래");
                    _normalizedActualPos.x = _portionX;
                    _normalizedActualPos.y = _portionY;

                }
                else
                {
                    // Debug.Log("0,1 위");
                    _normalizedActualPos.x = _portionX + 1;
                    _normalizedActualPos.y = _portionY + 1;
                }
            }
        }
        //_normalized_actual_pos.x = _portion_x;
        //_normalized_actual_pos.y = _portion_y;

        Vector2Int _logicalPos = new Vector2Int();
        int _logicalI = (int)((_normalizedActualPos.x + _normalizedActualPos.y) / 2f);
        int _logicalJ = (int)((_normalizedActualPos.y - _normalizedActualPos.x) / 2f);
        _logicalPos.x = _logicalI;
        _logicalPos.y = _logicalJ;

        //Debug.Log(_logical_pos);
        return _logicalPos;
    }

    public static Vector2 LogicalToActualPos(Vector2Int _logicalPos) {
        return new Vector2((_logicalPos.x - _logicalPos.y) * TILE_X_SIZE * 0.5f, (_logicalPos.x + _logicalPos.y) * TILE_Y_SIZE * 0.5f);
    }

    public void SetParentRaidTile(MyTile _tile)
    {
        if(raidChildTileList == null)
        {
            raidChildTileList = new List<MyTile>();
        }

        raidChildTileList.Add(_tile);
    }

    public void SetChildRaidTile(MyTile _tile)
    {
        raidParentTIle = _tile;
        TileType raidType = _tile.tileType;
        tileType = TileType.Raid;

        switch (raidType)
        {
            case TileType.Wall:
            case TileType.PackedItem:
                colliding = true;
                break;
            default:
                colliding = false;
                break;
        }

        if(tileObject != null)
        {
            tileObject.SetActive(false);
            tileObject = null;
        }
    }

    public void UnSetChildRaidTile()
    {
        raidParentTIle = null;
        tileType = TileType.Default;

        colliding = false;

        if (tileObject != null)
        {
            tileObject.SetActive(false);
            tileObject = null;
        }
    }

    public void OpenBox()
    {
        if (nowPackedItem == null)
        {
            return;
        }
        PackedItem _packItem = nowPackedItem;
        int level = _packItem.itemLevel;
        _packItem.UnpackItem();
        tileObject = GameObject.Instantiate(itemData.LoadObject(), _itemParent);
        tileObject.transform.position = actualPos;
        nowPackedItem = null;
       
        nowDroppedItem = new DroppedItem(logicalPos, tileObject, itemData, UnityEngine.Random.Range(1, 5));
        colliding = false;
        tileType = TileType.DroppedItem;
        if(raidChildTileList != null)
        {
            for (int i = 0; i < raidChildTileList.Count; i++)
            {
                raidChildTileList[i].colliding = false;
            }
        }
        
    }

    public void SetItem(ItemData _data)
    {
        tileObject = GameObject.Instantiate(ItemData.GetBoxObject(),_itemParent);
        tileObject.SetActive(false);
        colliding = true;
        tileObject.transform.position = actualPos;
        itemData = _data;
        //switch (_data.itemEnum)
        //{
        //    case ItemEnum.Sword:
        //    case ItemEnum.Hammer:
        //    case ItemEnum.Spear:
        //    case ItemEnum.Chain:
        //        nowNeturalObject = new PackedWeapon(logicalPos, tileObject, _data,UnityEngine.Random.Range(1,5));
        //        break;
        //    default:
        //        nowNeturalObject = new PackedCloth(logicalPos, tileObject, _data, UnityEngine.Random.Range(1, 5));
        //        break;
        //}

        nowPackedItem = new PackedItem(logicalPos, tileObject, _data, UnityEngine.Random.Range(1, 5));

        //이제 둥실둥실 해줘야함
    }

    public void EquipItem()
    {
        if(nowDroppedItem == null)
        {
            if(tileObject != null)
            {
                tileObject.SetActive(false);
            }
            return;
        }

        nowDroppedItem.DestroyObject();
        nowDroppedItem = null;
        itemData = null;
        tileType = TileType.Default;
        for(int i = 0; i < raidChildTileList.Count; i++)
        {
            raidChildTileList[i].UnSetChildRaidTile();
        }


    }

    public void DestroyObject()
    {
        if (tileObject != null)
        {
            GameObject.Destroy(tileObject);
            tileObject = null;
        }
        if(creep != null)
        {
            creep.DestroyObject();
        }
    }

    public void ActiveObject()
    {
        if (tileObject != null)
        {
            tileObject.SetActive(true);
        }
        if (creep != null)
        {
            creep.ActiveObject();
        }
    }
    
    public void SetCreep(Creep _creep)
    {
        creep = _creep;
    }



}
