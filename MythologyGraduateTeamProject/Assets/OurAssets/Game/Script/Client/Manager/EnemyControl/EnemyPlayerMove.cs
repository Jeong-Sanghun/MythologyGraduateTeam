using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPlayerMove : PlayerMove
{
    float _moveTimer = 0;
    Vector2 _originPos;
    Vector2 _destPos;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }


    public void SetPlayerNetworkMove(Vector2 _dest)
    {
        _moveTimer = 0;
        _originPos = _characterObject.transform.position;
        _destPos = _dest;
    }

    void PlayerNetworkMove()
    {
        _moveTimer += Time.deltaTime / NetworkFrameManager.TICK_SECOND;
        if (_moveTimer >= 1)
        {
            _moveTimer = 1;
        }
        _characterObject.transform.position = Vector2.Lerp(_originPos, _destPos, _moveTimer);

       
        
    }


    // Update is called once per frame
    void Update()
    {
        PlayerNetworkMove();
    }
}
