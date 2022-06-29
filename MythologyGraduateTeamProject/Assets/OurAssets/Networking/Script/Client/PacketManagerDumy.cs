using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacketManagerDumy : MonoBehaviour
{

    SendPacket _nowSendPacket;
    ReceivePacket _nowReceivePacket;
    [SerializeField]
    PlayerNetworkManager _playerNetworkManager;
    [SerializeField]
    EnemyNetworkManager _enemyNetworkManager;

    ReceivePacketWrapperDumy _localPacketWrapper;
    
    int _fileNumber;
    // Start is called before the first frame update
    void Start()
    {
        _fileNumber = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            _localPacketWrapper = new ReceivePacketWrapperDumy();
            _localPacketWrapper.packetList = new List<ReceivePacket>();
            Debug.Log("시작");
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            JsonManager.SaveJson<ReceivePacketWrapperDumy>(_localPacketWrapper, "PacketTest" + _fileNumber.ToString());
            _localPacketWrapper = null;
            Debug.Log("끝");
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Debug.Log("??");
            _enemyNetworkManager._packetWrapper = JsonManager.LoadSaveData<ReceivePacketWrapperDumy>("PacketTest0");
            
        }
    }

    public void AddPlayerContinuousData(PlayerContinuousData _data)
    {
        if(_localPacketWrapper == null)
        {
            return;
        }
        Debug.Log("기록중");
        List<ContinuousData> _dataList;
        if (_localPacketWrapper.packetList == null)
        {
            _localPacketWrapper.packetList = new List<ReceivePacket>();
        }
        if(_nowReceivePacket== null)
        {
            
            _nowReceivePacket = new ReceivePacket();
            _nowReceivePacket.continuousDataList = new List<ContinuousData>();
            _localPacketWrapper.packetList.Add(_nowReceivePacket);

        }
        ContinuousData _conData = new ContinuousData();
        _conData.playerContinuousData = _data;
        _nowReceivePacket.continuousDataList.Add(_conData);
    }

}
