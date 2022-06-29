using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyNetworkManager : MonoBehaviour
{
    public static EnemyNetworkManager singleton;
    [SerializeField]
    List<EnemyControl> _enemyControlList;
    [HideInInspector]
    public ReceivePacketWrapperDumy _packetWrapper;
    int _nowPacketIndex = 0;
    int _nowConDataIndex = 0;
    private void Awake()
    {
        if(singleton == null)
        {
            singleton = this;
            _packetWrapper = null;
        }
        else
        {
            Destroy(gameObject);
        }
        
    }


    public void OnFrameChange()
    {
        
        if(_packetWrapper == null)
        {
            return;
        }


        _nowConDataIndex++;
        if (_nowConDataIndex >= _packetWrapper.packetList[_nowPacketIndex].continuousDataList.Count)
        {
            _nowConDataIndex = 0;
            _nowPacketIndex++;
            if (_nowPacketIndex >= _packetWrapper.packetList.Count)
            {
                _packetWrapper = null;
                return;
            }
        }
        
        for (int i = 0; i < _enemyControlList.Count; i++)
        {
            _enemyControlList[i].OnFrameChange(_packetWrapper.packetList[_nowPacketIndex].continuousDataList[_nowConDataIndex].playerContinuousData);
        }
    }
}
