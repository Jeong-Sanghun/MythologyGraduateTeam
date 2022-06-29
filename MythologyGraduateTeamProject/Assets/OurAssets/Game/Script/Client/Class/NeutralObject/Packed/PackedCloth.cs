using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PackedCloth : PackedItem
{

    public int clothLevel;

    public PackedCloth(Vector2Int _pos, GameObject _object, ItemData _data, int _level) : base(_pos, _object, _data,_level)
    {
        clothLevel = _level;
    }
}
