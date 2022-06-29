using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    [SerializeField]
    int _playerId;
    [SerializeField]
    ClientPlayerMove _playerMove;
    [SerializeField]
    PlayerAnimation _playerAnimation;
    [SerializeField]
    PlayerNetworkManager _playerNetworkManager;
    //[SerializeField]
    //PlayerEquipment _playerAnimationOverride;
    //[SerializeField]
    //PlayerEquipment _playerEquipment;
    [SerializeField]
    ItemManager _itemManager;

    private bool _playerAttacking;

    void Update()
    {
        MoveInputUpdate();

        AttackInputUpdate();



    }

    public void OnFrameUpdate()
    {
        
        _playerNetworkManager.AddPlayerContinuousData(_playerId, _playerAnimation.nowAnim, _playerMove.GetCharacterActualPos(), _playerMove.GetRotation());
    }

    public Vector2Int GetPlayerLogicalPos()
    {
        return MyTile.ActualToLogicalPosition(_playerMove.GetCharacterActualPos());
    }

    public Vector2 GetPlayerActualPos()
    {
        return _playerMove.GetCharacterActualPos();
    }

    public void OnMapChange()
    {
        _playerMove.OnMapChange();
    }


    void AttackInputUpdate()
    {
        //좌클릭 공격 수정함 feat.석희
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
        if (Input.GetKeyDown(KeyCode.S))
        {
            _playerMove.SetKeyboardMove(KeyboardArrow.Down, true);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            _playerMove.SetKeyboardMove(KeyboardArrow.Left, true);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            _playerMove.SetKeyboardMove(KeyboardArrow.Right, true);
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            _playerMove.SetKeyboardMove(KeyboardArrow.Up, true);
        }

        if (Input.GetKeyUp(KeyCode.S))
        {
            _playerMove.SetKeyboardMove(KeyboardArrow.Down, false);
        }
        if (Input.GetKeyUp(KeyCode.A))
        {
            _playerMove.SetKeyboardMove(KeyboardArrow.Left, false);
        }
        if (Input.GetKeyUp(KeyCode.D))
        {
            _playerMove.SetKeyboardMove(KeyboardArrow.Right, false);
        }
        if (Input.GetKeyUp(KeyCode.W))
        {
            _playerMove.SetKeyboardMove(KeyboardArrow.Up, false);
        }

        if (Input.GetMouseButtonDown(1))
        {
            _playerMove.OnMouseRightClickDown(Input.mousePosition);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            _itemManager.OpenBox(_playerMove.GetCollidingTilesInRadius());
            _playerMove.Decollide();
            
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            _itemManager.EquipItem(_playerMove.nowStandTile);
            Debug.Log("키다운 F");
            _playerMove.Decollide();
        }

        _playerMove.Move();
    }
}
