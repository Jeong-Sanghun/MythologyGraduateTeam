using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    [HideInInspector]
    public ItemDataWrapper itemDataWrapper;


    [SerializeField]
    TileMapGenerator _tileMapGenerator;
    [SerializeField]
    ItemSpawn _itemSpawn;
    [SerializeField]
    ItemOpenEquipper _itemOpenEquipper;
    List<MyTile> _itemTileList;

    //다른 아이템 짜잘매니저에서 써야함
    public Dictionary<Vector2Int, MyTile> tileDictionary;
    // Start is called before the first frame update
    void Awake()
    {

        itemDataWrapper = JsonManager.LoadSaveData<ItemDataWrapper>("ItemDataWrapper");
    }
    void Start()
    {
        tileDictionary = _tileMapGenerator.tileDictionary;

    }

    public void OnMapChange()
    {
        if (_itemTileList == null)
        {
            _itemTileList = new List<MyTile>();
        }
        else
        {
            _itemTileList.Clear();
        }
    }


    public void SpawnItem(MyTile _tile)
    {
        _itemTileList.Add(_tile);
        _itemSpawn.SpawnItem(_tile);
    }

    public void OpenBox(List<MyTile> _tileList)
    {
        Debug.Log(_tileList.Count);
        for (int i = 0; i < _tileList.Count; i++)
        {
            _itemOpenEquipper.OpenItem(_tileList[i]);
        }

    }

    public bool EquipItem(MyTile _tile)
    {
        _itemOpenEquipper.EquipItem(_tile);

        return true;
    }

    private MyTile GetTileWithActualPos(Vector2 _pos)
    {
        return tileDictionary[MyTile.ActualToLogicalPosition(_pos)];
    }


}
