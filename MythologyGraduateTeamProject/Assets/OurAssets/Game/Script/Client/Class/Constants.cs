using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Constants
{
    public static int PATH_FINDING_RANGE;
    public static float TILE_X_SIZE;
    public static float TILE_Y_SIZE;

    public int pathFindingRange;
    public float tileXSize;
    public float tileYSize;

    public void SetStaticValue()
    {
        PATH_FINDING_RANGE = pathFindingRange;
        TILE_X_SIZE = tileXSize;
        TILE_Y_SIZE = tileYSize;

    }
}
