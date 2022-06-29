using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PackedWeapon : PackedItem
{
    public int weaponLevel;

    public PackedWeapon(Vector2Int _pos, GameObject _object, ItemData _data,int _level) : base(_pos, _object, _data,_level)
    {
        weaponLevel = _level;
    }
}
