using UnityEngine;

public class PlayerAnimationEvent : MonoBehaviour
{
    private WeaponData currentClassWeapon;   //현재 장착한 무기
    private bool isAttakedEnemy;
    private bool isAttack = false;    //현재 공격 중인지
    private int howManyWeapons = System.Enum.GetValues(typeof(WeaponName)).Length;
    float timer = 0;
    [SerializeField]
    Animator Animator;


    //IEnumerator AttackCourse()
    //{
    //    Debug.Log("공격선딜레이애니메이션");
    //    while (timer < currentClassWeapon.attackDelayStart)     //선딜레이 기다리기
    //    {
    //        timer += Time.deltaTime;   //60분의 1초를 계속 더함
    //        if (isAttakedEnemy)
    //        {
    //            Debug.Log("넉백 당함");     //넉백 함수 필요
    //            yield break;
    //        }
    //        yield return null;   //60분의 1초 기다리겠다
    //    }

    //    Debug.Log("공격중애니메이션");
    //    timer = 0;
    //    while (timer < currentClassWeapon.attackDelayStart)     //공격 지속중
    //    {
    //        timer += Time.deltaTime;
    //        if (isAttakedEnemy)
    //        {
    //            Debug.Log("넉백 당함");    //넉백 함수 필요
    //            yield break;
    //        }
    //        yield return null;
    //    }
    //    Debug.Log("공격후딜레이");
    //    timer = 0;
    //    while (timer < currentClassWeapon.attackDelayFinish)    //후딜레이 기다리기
    //    {
    //        timer += Time.deltaTime;
    //        if (isAttakedEnemy)
    //        {
    //            Debug.Log("넉백 당함");    //넉백

    //        }
    //        yield return null;
    //    }
    //    Debug.Log("맞고있는지에 대한 여부를 체크");

    //}



    public void PlayerAttackStart()   //animator event 삽입용1
    {
        Debug.Log("공격판정이 나오는 함수");  //공격판정 유무 구분용도로 Bool변수 하나 추가
        isAttack = true;
    }

    public void PlayerAttackEnd()   //animator event 삽입용2
    {
        Debug.Log("공격판정이 끝나는 함수");  //공격판정 유무 구분용도로 Bool변수 하나 추가
        isAttack = false;
    }

    public void PlayerAttacking()   //animator event 삽입용3
    {
    }



    public void SaveWeaponData()   //json으로 WeaponData를 저장
    {
        WeaponDataWrapper _weaponWrapper = new WeaponDataWrapper();
        WeaponData[] _classWeapon = new WeaponData[howManyWeapons];
        for (int i = 0; i < howManyWeapons; i++)
        {
            _classWeapon[i] = new WeaponData();
            _classWeapon[i].SetWeaponData(WeaponName.Hand, 0, 0, 0);
        }

        _weaponWrapper.weaponDataArray = _classWeapon;
        JsonManager.SaveJson<WeaponDataWrapper>(_weaponWrapper, "ClassWeaponWrapper");
    }

    // Start is called before the first frame update
    void Start()
    {
        SaveWeaponData();
    }

    // Update is called once per frame
    void Update()
    {

    }
}