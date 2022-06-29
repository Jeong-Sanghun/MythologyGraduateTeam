using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMove : MonoBehaviour
{
    protected ManagerGroup _dataCenter;
    [SerializeField]
    protected GameObject _characterObject;

    [SerializeField]
    protected GameObject _modelObject;

    protected Dictionary<Vector2Int, MyTile> _tileDictionary;
    protected float[] _speedArray;
    protected int _speedIndex;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        _dataCenter = ManagerGroup.singleton;
        _tileDictionary = _dataCenter.tilemapGenerator.tileDictionary;

        _speedArray = new float[] { 0.5f, 1f, 2f };
        _speedIndex = 0;


    }
    public virtual Vector2 GetCharacterActualPos()
    {
        return _characterObject.transform.position;
    }

    public virtual float GetRotation()
    {
        return _modelObject.transform.localEulerAngles.y;
    }

    public virtual void OnFrameChange()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
