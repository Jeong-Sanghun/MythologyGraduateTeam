using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ReceivePacket
{
    public List<ContinuousData> continuousDataList;
    public List<PeriodicData> periodicDataList;
}
