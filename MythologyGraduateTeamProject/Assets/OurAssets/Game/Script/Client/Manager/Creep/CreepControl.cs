using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreepControl : MonoBehaviour
{
    [SerializeField]
    PlayerControl _playerControl;
    [SerializeField]
    Transform _creepParent;
    CreepData _nowCreepData;

    List<Creep> _nowCreepList;
    
    // Start is called before the first frame update
    void Start()
    {
        _nowCreepData = new CreepData();
        _nowCreepData.creepName = CreepName.Hydra;
        _nowCreepList = new List<Creep>();
        StartCoroutine(CreepAnimationCoroutine());
    }

    public void OnMapChange()
    {
        _nowCreepList.Clear();
    }

    IEnumerator CreepAnimationCoroutine()
    {
        WaitForSeconds _second = new WaitForSeconds(0.5f);
        while (true)
        {
            yield return _second;
            for (int i = 0; i < _nowCreepList.Count; i++)
            {
                _nowCreepList[i].CheckAttack(_playerControl.GetPlayerActualPos());
            }
        }
    }

    public void SpawnCreep(MyTile _tile,bool _onMapChange)
    {
        Creep _creep = new Creep(_nowCreepData.LoadObject(_tile.tileEnvironment), _creepParent, _tile,_onMapChange);
        _nowCreepList.Add(_creep);
    }



    // Update is called once per frame
    void Update()
    {
        
    }
}
