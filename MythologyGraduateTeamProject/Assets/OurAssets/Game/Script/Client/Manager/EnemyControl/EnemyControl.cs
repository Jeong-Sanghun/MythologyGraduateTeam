using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyControl : MonoBehaviour
{
    [SerializeField]
    PlayerAnimation _playerAnimation;
    [SerializeField]
    EnemyPlayerMove _playerMove;
    //[SerializeField]
    //PlayerEquipment _playerAnimationOverride;
    //[SerializeField]
    //PlayerEquipment _playerEquipment;
    [SerializeField]
    ItemManager _itemManager;

    EnemyNetworkManager _enemyNetworkManager;
    

    private bool _playerAttacking;

    private void Start()
    {
        _enemyNetworkManager = EnemyNetworkManager.singleton;
    }

    public void OnFrameChange(PlayerContinuousData _enemyData)
    {
        Debug.Log(_enemyData.worldPosition);
        _playerMove.SetPlayerNetworkMove(_enemyData.worldPosition);
    }


    void AttackInputUpdate()
    {
        //ÁÂÅ¬¸¯ °ø°Ý ¼öÁ¤ÇÔ feat.¼®Èñ
        if (Input.GetMouseButtonDown(0))
        {
            _playerAttacking = true;
        }
        else _playerAttacking = false;

        if (_playerAttacking == false)
        {
            _playerAnimation.SetAnimation(PlayerAnimationEnum.Attack, false);
        }
        if (_playerAttacking == true)
        {
            _playerAnimation.SetAnimation(PlayerAnimationEnum.Attack, true);
        }
    }

    void MoveInputUpdate()
    {
       
    }
}