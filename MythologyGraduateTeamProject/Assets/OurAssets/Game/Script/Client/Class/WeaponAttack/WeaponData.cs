using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public enum WeaponName {Hand, Sword, Hammer, Spear, Chain}
public enum ClothName { None,Hat,Shoes }

[System.Serializable]
public class WeaponData
{
    //무기 소지 여부
    public WeaponName nowWeapon;
    public float attackRadius;   //공격반지름
    public float attackAngle;   //공격각도
    public float attackDamage;    //공격데미지
    public static int weaponCount;

    public WeaponData()
    {
        nowWeapon = WeaponName.Hand;
        attackRadius = 0;
        attackAngle = 0;
        attackDamage = 0;

        if(weaponCount == 0)
        {
            weaponCount = Enum.GetValues(typeof(WeaponName)).Length;
        }
    }

    public void SetWeaponData(WeaponName _weapon, float _radius, float _angle, float _damage)
    {
        nowWeapon = _weapon;
        attackRadius = _radius;
        attackAngle = _angle;
        attackDamage = _damage;
    }
}
