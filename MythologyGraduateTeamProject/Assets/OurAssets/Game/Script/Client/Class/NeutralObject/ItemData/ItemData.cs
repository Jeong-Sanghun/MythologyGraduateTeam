using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemEnum
{
    Hand,Sword, Hammer, Spear, Chain,Hat, Shoes
}

[System.Serializable]
public class ItemData
{
    public ItemEnum itemEnum;
    public int probability;
    [System.NonSerialized]
    Sprite _itemSprite;
    [System.NonSerialized]
    GameObject _itemObject;
    [System.NonSerialized]
    static GameObject _boxObject;

    public ItemData()
    {
        probability = 10;
    }

    public Sprite LoadSprite()
    {
        if(_itemSprite == null)
        {
            _itemSprite = Resources.Load<Sprite>("ItemSprite/" + itemEnum.ToString());
        
        }
        return _itemSprite;
    }

    public GameObject LoadObject()
    {
        if (_itemObject == null)
        {
            _itemObject = Resources.Load<GameObject>("ItemPrefab/" + itemEnum.ToString());

        }
        return _itemObject;
    }

    public static GameObject GetBoxObject()
    {
        if (_boxObject == null)
        {
            _boxObject = Resources.Load<GameObject>("ItemPrefab/Box");

        }
        return _boxObject;
    }
}
