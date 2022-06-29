using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEquipment : MonoBehaviour
{
    [SerializeField]
    Animator animator;
    [SerializeField]
    AnimationClip[] weaponAnimationClip;
    [SerializeField]
    AnimatorOverrideController animatorOverrideController;
    [SerializeField]
    Transform hand;
    [SerializeField]
    GameObject[] weapon;

    GameObject nowHandedWeapon;
    GameObject nowWearingCloth; //상훈

    public void EquipWeapon(ItemData _data)
    {
        //for (int i = 0; i < weapon.Length + 1; i++)
        //{
        //    Destroy(weapon[i]);
        //}
        
        if(nowHandedWeapon != null){
            Destroy(nowHandedWeapon);
        }
        
        nowHandedWeapon = Instantiate(_data.LoadObject(), hand);
        nowHandedWeapon.transform.localScale *= 5;
        //animator.SetInteger("What_weapon", (int)name);  //애니메이터에 int파라미터 What_weapon을 변경
        animatorOverrideController["Attack_Punch"] = weaponAnimationClip[(int)_data.itemEnum];
    }

    //상훈
    void EquipCloth(ClothName name)
    {

    }




    // Start is called before the first frame update
    void Start()
    {
        //animator = GetComponent<Animator>();  //인스펙터에 animator를 초기화 하는 함수인듯
        //animatorOverrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
        //animator.runtimeAnimatorController = animatorOverrideController;  //동적할당
        animatorOverrideController["Attack_Punch"] = weaponAnimationClip[0];  //기본공격으로 초기화
        nowHandedWeapon = Instantiate(weapon[0], hand);
    }

    // Update is called once per frame
    //void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.Alpha1))
    //    {
    //        Debug.Log("무기1장착");
    //        Equip_weapon(WeaponName.Hand);
    //    }
    //    else if (Input.GetKeyDown(KeyCode.Alpha2))
    //    {
    //        Debug.Log("무기2장착");
    //        Equip_weapon(WeaponName.Sword);
    //    }
    //    else if (Input.GetKeyDown(KeyCode.Alpha3))
    //    {
    //        Debug.Log("무기3장착");
    //        Equip_weapon(WeaponName.Hammer);
    //    }
    //    else if (Input.GetKeyDown(KeyCode.Alpha4))
    //    {
    //        Debug.Log("무기4장착");
    //        Equip_weapon(WeaponName.Spear);
    //    }
    //}
}
