using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemOpenEquipper : MonoBehaviour
{
    [SerializeField]
    ItemManager _itemManager;
    [SerializeField]
    PlayerEquipment _playerEquipment;
    Dictionary<Vector2Int, MyTile> _tileDictionary;

    private void Start()
    {
        _tileDictionary = _itemManager.tileDictionary;
    }

    public void OpenItem(MyTile _tile)
    {
        if(_tile == null)
        {
            return;
        }
        MyTile _packedTile = CheckPackedItemTileWithActualPos(_tile);
        if (_packedTile == null)
        {
            return;
        }
        _packedTile.OpenBox();
    }

    public void EquipItem(MyTile _tile)
    {
        MyTile _droppedTile = CheckDroppedItemTileWithActualPos(_tile);
        if(_droppedTile == null)
        {
            return;
        }
        Debug.Log("üũ");
        ItemData _data = _droppedTile.itemData;
        _droppedTile.EquipItem();
        _playerEquipment.EquipWeapon(_data);
        
    }

    public MyTile CheckDroppedItemTileWithActualPos(MyTile _tile)
    {
        Debug.Log("üũ");
        if (_tile == null)
        {
            Debug.Log("üũ");
            return null;
        }
        if (_tile.tileType != TileType.DroppedItem && _tile.tileType != TileType.Raid)
        {
            Debug.Log("üũ");
            return null;
        }
        if (_tile.tileType == TileType.Raid)
        {
            if (_tile.raidParentTIle.tileType == TileType.DroppedItem)
            {
                Debug.Log("üũ");
                return _tile.raidParentTIle;
            }
            else
            {
                Debug.Log("üũ");
                return null;
            }
        }
        if (_tile.tileType == TileType.DroppedItem)
        {
            Debug.Log("üũ");
            return _tile;
        }
        else
        {
            Debug.Log("üũ");
            return null;
        }
    }

    public MyTile CheckPackedItemTileWithActualPos(MyTile _tile)
    {
        if (_tile == null)
        {

            return null;
        }
        if (_tile.tileType != TileType.PackedItem && _tile.tileType != TileType.Raid)
        {

            return null;
        }
        if (_tile.tileType == TileType.Raid)
        {
            if (_tile.raidParentTIle.tileType == TileType.PackedItem)
            {

                return _tile.raidParentTIle;
            }
            else
            {
                return null;
            }
        }
        if (_tile.tileType == TileType.PackedItem)
        {
            return _tile;
        }
        else
        {
            return null;
        }
    }
}
