using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroppedWeapon : DroppedItem
{
    public WeaponName weaponname;
    public int weaponLevel;
    public DroppedWeapon(Vector2Int _pos, GameObject _object, ItemData _data,int _level) : base(_pos, _object, _data,_level)
    {

    }
}
