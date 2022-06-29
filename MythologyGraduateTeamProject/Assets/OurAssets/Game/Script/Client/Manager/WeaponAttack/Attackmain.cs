using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AttackMain : MonoBehaviour
{
    public GameObject weapon1;



    // Start is called before the first frame update
    void Start()
    {
        Instantiate(weapon1, new Vector3 (1.2f, 2.7f, 0f), Quaternion.identity);
 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
