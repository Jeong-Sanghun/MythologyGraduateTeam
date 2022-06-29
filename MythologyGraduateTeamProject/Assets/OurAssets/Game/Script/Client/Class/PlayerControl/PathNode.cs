using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode
{
    public Vector2 actualPos;
    public MyTile tile;
    public float fValue;
    public float gValue;
    public float hValue;
    public PathNode parentNode;

    public PathNode(Vector2 _pos, MyTile _tile,PathNode _parent, float _hValue, float _gValue)
    {
        actualPos = _pos;
        tile = _tile;
        parentNode = _parent;
        gValue = _gValue;
        hValue = _hValue;
        fValue = gValue + hValue;
    }

    public void UpdateValues(float _hValue, float _gValue)
    {
        float _fValue = _gValue+ _hValue;
        if (_fValue < fValue)
        {
            fValue = _fValue;
        }
    }

    public void UpdateValues(float _hValue, float _gValue,PathNode _parent)
    {
        float _fValue = _gValue + _hValue;
        
        if (_fValue < fValue)
        {
            parentNode = _parent;
            fValue = _fValue;
        }
    }
}
