using UnityEngine;

public class PlayerAnimationEvent : MonoBehaviour
{
    private WeaponData currentClassWeapon;   //���� ������ ����
    private bool isAttakedEnemy;
    private bool isAttack = false;    //���� ���� ������
    private int howManyWeapons = System.Enum.GetValues(typeof(WeaponName)).Length;
    float timer = 0;
    [SerializeField]
    Animator Animator;


    //IEnumerator AttackCourse()
    //{
    //    Debug.Log("���ݼ������ִ̾ϸ��̼�");
    //    while (timer < currentClassWeapon.attackDelayStart)     //�������� ��ٸ���
    //    {
    //        timer += Time.deltaTime;   //60���� 1�ʸ� ��� ����
    //        if (isAttakedEnemy)
    //        {
    //            Debug.Log("�˹� ����");     //�˹� �Լ� �ʿ�
    //            yield break;
    //        }
    //        yield return null;   //60���� 1�� ��ٸ��ڴ�
    //    }

    //    Debug.Log("�����߾ִϸ��̼�");
    //    timer = 0;
    //    while (timer < currentClassWeapon.attackDelayStart)     //���� ������
    //    {
    //        timer += Time.deltaTime;
    //        if (isAttakedEnemy)
    //        {
    //            Debug.Log("�˹� ����");    //�˹� �Լ� �ʿ�
    //            yield break;
    //        }
    //        yield return null;
    //    }
    //    Debug.Log("�����ĵ�����");
    //    timer = 0;
    //    while (timer < currentClassWeapon.attackDelayFinish)    //�ĵ����� ��ٸ���
    //    {
    //        timer += Time.deltaTime;
    //        if (isAttakedEnemy)
    //        {
    //            Debug.Log("�˹� ����");    //�˹�

    //        }
    //        yield return null;
    //    }
    //    Debug.Log("�°��ִ����� ���� ���θ� üũ");

    //}



    public void PlayerAttackStart()   //animator event ���Կ�1
    {
        Debug.Log("���������� ������ �Լ�");  //�������� ���� ���п뵵�� Bool���� �ϳ� �߰�
        isAttack = true;
    }

    public void PlayerAttackEnd()   //animator event ���Կ�2
    {
        Debug.Log("���������� ������ �Լ�");  //�������� ���� ���п뵵�� Bool���� �ϳ� �߰�
        isAttack = false;
    }

    public void PlayerAttacking()   //animator event ���Կ�3
    {
    }



    public void SaveWeaponData()   //json���� WeaponData�� ����
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