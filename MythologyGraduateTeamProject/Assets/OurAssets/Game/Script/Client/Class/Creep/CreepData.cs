using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CreepData
{
    public CreepName creepName;
    GameObject[] creepPrefabArray;

    public GameObject LoadObject(TileEnvironment _environment)
    {
        int _index = (int)_environment;
        if (creepPrefabArray == null)
        {
            creepPrefabArray = new GameObject[System.Enum.GetValues(typeof(TileEnvironment)).Length];

        }
        if (creepPrefabArray[_index] == null)
        {
            creepPrefabArray[_index ]= Resources.Load<GameObject>("CreepPrefab/" +_environment.ToString()+ creepName.ToString());

        }
        return creepPrefabArray[_index];
    }
}
