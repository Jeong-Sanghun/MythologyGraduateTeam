using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeutralObject
{
    public Vector2Int logicalPos;
    public GameObject obj;
    public ItemData itemData;

    public NeutralObject(Vector2Int _pos, GameObject _object, ItemData _data)
    {
        logicalPos = _pos;
        obj = _object;
        itemData = _data;
    }

    public void DestroyObject()
    {
        obj.SetActive(false);
    }

    public void Vibrate(float timer)
    {

    }


}
