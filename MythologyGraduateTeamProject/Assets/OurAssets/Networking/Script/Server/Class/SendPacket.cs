using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SendPacket
{
    public int nowFrame;
    public int playerId;
    public PeriodicData periodicData;
    public ContinuousData continuousData;


}
