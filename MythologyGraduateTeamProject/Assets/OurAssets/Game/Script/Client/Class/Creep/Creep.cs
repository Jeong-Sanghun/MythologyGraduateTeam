using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creep
{
    public MyTile _parentTile;
    public Vector2Int logicalPos;
    CreepAnimation _nowAnimation;
    Animator _animator;
    GameObject _creepObject;
    Rigidbody2D _rigidbody;

    public Creep(GameObject _prefab,Transform _parent, MyTile _tile,bool _onMapChange)
    {
        _creepObject = GameObject.Instantiate(_prefab, _parent);
        _parentTile = _tile;
        _creepObject.transform.position = _tile.actualPos;
        _animator = _creepObject.GetComponent<Animator>();
        _rigidbody = _creepObject.GetComponent<Rigidbody2D>();
        _nowAnimation = CreepAnimation.Idle;
        _creepObject.SetActive(!_onMapChange);
        _tile.SetCreep(this);
    }

    void SetAnimation(CreepAnimation _anim, bool _active)
    {
        _animator.SetBool(_anim.ToString(), _active);
    }

    public void CheckAttack(Vector2 _playerPos)
    {
        if(_creepObject == null)
        {
            return;
        }
        Vector2 _creepPos = _creepObject.transform.position;
        if( (_creepPos- _playerPos).sqrMagnitude <10)
        {
            SetAnimation(CreepAnimation.Attack, true);
        }
        else
        {
            SetAnimation(CreepAnimation.Attack, false);
        }
    }

    public void DestroyObject()
    {
        GameObject.Destroy(_creepObject);
    }

    public void ActiveObject()
    {
        _creepObject.SetActive(true);
    }
    

}
