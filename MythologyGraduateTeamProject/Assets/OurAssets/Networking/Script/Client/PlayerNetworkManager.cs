using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNetworkManager : MonoBehaviour
{

    [SerializeField]
    PacketManagerDumy _packetManagerDumy;

    public void AddPlayerContinuousData(int _id, PlayerAnimationEnum _state, Vector2 _pos, float _rot)
    {
        PlayerContinuousData _data = new PlayerContinuousData(_id, _state, _pos, _rot);
        _packetManagerDumy.AddPlayerContinuousData(_data);
    }
}
