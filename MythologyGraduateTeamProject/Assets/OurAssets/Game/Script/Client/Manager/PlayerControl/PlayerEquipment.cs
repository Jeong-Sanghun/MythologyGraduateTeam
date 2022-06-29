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
    GameObject nowWearingCloth; //����

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
        //animator.SetInteger("What_weapon", (int)name);  //�ִϸ����Ϳ� int�Ķ���� What_weapon�� ����
        animatorOverrideController["Attack_Punch"] = weaponAnimationClip[(int)_data.itemEnum];
    }

    //����
    void EquipCloth(ClothName name)
    {

    }




    // Start is called before the first frame update
    void Start()
    {
        //animator = GetComponent<Animator>();  //�ν����Ϳ� animator�� �ʱ�ȭ �ϴ� �Լ��ε�
        //animatorOverrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
        //animator.runtimeAnimatorController = animatorOverrideController;  //�����Ҵ�
        animatorOverrideController["Attack_Punch"] = weaponAnimationClip[0];  //�⺻�������� �ʱ�ȭ
        nowHandedWeapon = Instantiate(weapon[0], hand);
    }

    // Update is called once per frame
    //void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.Alpha1))
    //    {
    //        Debug.Log("����1����");
    //        Equip_weapon(WeaponName.Hand);
    //    }
    //    else if (Input.GetKeyDown(KeyCode.Alpha2))
    //    {
    //        Debug.Log("����2����");
    //        Equip_weapon(WeaponName.Sword);
    //    }
    //    else if (Input.GetKeyDown(KeyCode.Alpha3))
    //    {
    //        Debug.Log("����3����");
    //        Equip_weapon(WeaponName.Hammer);
    //    }
    //    else if (Input.GetKeyDown(KeyCode.Alpha4))
    //    {
    //        Debug.Log("����4����");
    //        Equip_weapon(WeaponName.Spear);
    //    }
    //}
}
