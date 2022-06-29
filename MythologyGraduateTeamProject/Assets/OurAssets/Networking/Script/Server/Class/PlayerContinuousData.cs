using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerContinuousData
{
    public int playerId;
    //키값으로 쓸 게 뭔지 몰라서.
    public PlayerAnimationEnum playerState;
    public Vector2 worldPosition;
    public float rotation;

    public PlayerContinuousData(int _id, PlayerAnimationEnum _state, Vector2 _pos, float _rot)
    {
        playerId = _id;
        playerState = _state;
        worldPosition = _pos;
        rotation = _rot;
    }

}
