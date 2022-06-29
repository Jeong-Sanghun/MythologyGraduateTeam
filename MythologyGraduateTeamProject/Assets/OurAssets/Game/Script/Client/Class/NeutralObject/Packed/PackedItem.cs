using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PackedItem : NeutralObject
{
    public int itemLevel;
    public PackedItem(Vector2Int _pos, GameObject _object, ItemData _data,int _level):base(_pos, _object,_data)
    {
        itemLevel = _level;
    }

    public void UnpackItem()
    {
        DestroyObject();
    }
}
