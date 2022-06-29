using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkControl : MonoBehaviour
{
    public static NetworkControl singleton;
    public NetworkFrameManager networkFrameManager;


    private void Awake()
    {
        if(singleton == null)
        {
            singleton = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void OnEveryFrame(SendPacket incomePacket)
    {
        if(incomePacket == null)
        {
            return;
        }
    }




}
