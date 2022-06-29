using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawn : MonoBehaviour
{

    [SerializeField]
    ItemManager _itemManager;
    [SerializeField]
    GameObject _boxPrefab;
    [SerializeField]
    Transform _itemParent;

    ItemDataWrapper _itemDataWrapper;

    Dictionary<Vector2Int, MyTile> _tileDictionary;

    private void Awake()
    {
        MyTile.SetItemManager(_itemManager,_itemParent);
    }

    // Start is called before the first frame update
    void Start()
    {
        _tileDictionary = _itemManager.tileDictionary;
        _itemDataWrapper = _itemManager.itemDataWrapper;

    }


    public void SpawnItem(MyTile _tile)
    {
        Vector2Int _pivotTileLogicalPos = _tile.logicalPos;
        if (_tile.tileType == TileType.Raid)
        {
            return;
        }
        for(int i = 0; i < _tile.tileData.raidSize; i++)
        {
            for(int j = 0; j < _tile.tileData.raidSize; j++)
            {
                Vector2Int _childTilePos = _tile.logicalPos + new Vector2Int(i, j);
                MyTile _childTile;
                if (!_itemManager.tileDictionary.TryGetValue(_childTilePos, out _childTile))
                {
                    continue;
                }
                
                
                _tile.SetParentRaidTile(_childTile);
                if (i == 0 && j == 0)
                {
                    continue;
                }
                _childTile.SetChildRaidTile(_tile);

            }
        }

        _tile.SetItem(_itemManager.itemDataWrapper.GetRandomItem());
        



        
    }
}
