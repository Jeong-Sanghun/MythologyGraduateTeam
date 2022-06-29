using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TileGenerateDataWrapper
{
    public TileGenerateData[] tileGenerateDataArray;
    

    private int _wholeProbability;

    public TileGenerateDataWrapper(){
        _wholeProbability = 0;
    }

    public void SetNoneLoadedData()
    {
        for (int i = 0; i < tileGenerateDataArray.Length; i++)
        {
            tileGenerateDataArray[i].SetNoneLoadedData();
        }
    }

    public TileGenerateData GetRandomTile()
    {
        TileGenerateData _returnee = null;
        if (_wholeProbability == 0)
        {
            for(int i = 0; i < tileGenerateDataArray.Length; i++)
            {
                _wholeProbability += tileGenerateDataArray[i].probability;
            }
        }
        int _nowTracingProbability = 0;
        int _randomNum = Random.Range(0, _wholeProbability);
        while(_returnee == null)
        {
            for (int i = 0; i < tileGenerateDataArray.Length; i++)
            {
                _nowTracingProbability += tileGenerateDataArray[i].probability;
                if (_randomNum <= _nowTracingProbability)
                {
                    _returnee = tileGenerateDataArray[i];
                    return tileGenerateDataArray[i];
                }
            }
        }
        return _returnee;
    }

    public TileGenerateData GetDefaultTile()
    {
        return tileGenerateDataArray[0];
    }
}
