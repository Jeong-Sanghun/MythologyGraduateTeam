using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerGroup : MonoBehaviour
{
    public static ManagerGroup singleton;

    [HideInInspector]
    public TileMapGenerator tilemapGenerator;

    private void Awake()
    {
        if(singleton== null)
        {
            singleton = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        Constants _const = JsonManager.LoadSaveData<Constants>("Constants");
        _const.SetStaticValue();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
