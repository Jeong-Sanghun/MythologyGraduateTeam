using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class NetworkFrameManager : MonoBehaviour
{
    NetworkControl networkControl;
    public const float TICK_SECOND = 0.1f;

    public UnityEvent onFrameChangeEvent;
    //서버에서 해줄거.
    public uint frame;
    public void Start()
    {
        networkControl = NetworkControl.singleton;
        StartCoroutine(FrameCounter());
        //onFrameChangeEvent = new UnityEvent();
    }

    IEnumerator FrameCounter()
    {
        WaitForSeconds tick = new WaitForSeconds(TICK_SECOND);
        frame = 0;
        //나중에 서버에서 해줄 역할임.
        while (true)
        {
            frame++;
            onFrameChangeEvent.Invoke();
            yield return tick;
        }
    }





}
