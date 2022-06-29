using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemDataWrapper
{
    public ItemData[] itemDataArray;
    private int _wholeProbability;

    public ItemDataWrapper()
    {
        _wholeProbability = 0;
    }

    public ItemData GetRandomItem()
    {
        ItemData _returnee = null;
        if (_wholeProbability == 0)
        {
            for (int i = 0; i < itemDataArray.Length; i++)
            {
                _wholeProbability += itemDataArray[i].probability;
            }
        }
        int _nowTracingProbability = 0;
        int _randomNum = Random.Range(0, _wholeProbability);
        while (_returnee == null)
        {
            for (int i = 0; i < itemDataArray.Length; i++)
            {
                _nowTracingProbability += itemDataArray[i].probability;
                if (_randomNum <= _nowTracingProbability)
                {
                    _returnee = itemDataArray[i];
                    return itemDataArray[i];
                }
            }
        }
        return _returnee;
    }
}
