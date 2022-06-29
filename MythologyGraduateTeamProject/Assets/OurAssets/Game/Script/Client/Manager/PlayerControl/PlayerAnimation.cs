using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerAnimationEnum
{
    Run,Attack,Attacked
}

public class PlayerAnimation : MonoBehaviour
{

    [SerializeField]
    private Animator _playerAnimator;
    public PlayerAnimationEnum nowAnim;
    bool[] _stateArray;

    private void Start()
    {
        _stateArray = new bool[System.Enum.GetValues(typeof(PlayerAnimationEnum)).Length];
        for(int i = 0; i < _stateArray.Length; i++)
        {
            _stateArray[i] = false;
        }
    }

    public void SetAnimation(PlayerAnimationEnum anim, bool state)
    {
        
        _playerAnimator.SetBool(anim.ToString(), state);
        _stateArray[(int)anim] = state;
        if (state == false)
        {
            if(_stateArray[(int)PlayerAnimationEnum.Run] == true)
            {
                nowAnim = PlayerAnimationEnum.Run;
            }
            else
            {
                nowAnim = anim;
            }
        }
       

    }

    
}
