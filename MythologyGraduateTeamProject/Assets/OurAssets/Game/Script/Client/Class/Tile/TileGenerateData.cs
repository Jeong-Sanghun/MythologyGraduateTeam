using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TileGenerateData
{
    public int probability;
    public string tileTypeString;
    public int raidSize;

    [System.NonSerialized]
    public Sprite[] tileSpriteArray;
    [System.NonSerialized]
    public TileType tileType;
    public TileGenerateData()
    {
        tileTypeString = "Default";
        tileType = TileType.Default;
        probability = 10;
        raidSize = 1;
        tileSpriteArray = new Sprite[System.Enum.GetValues(typeof(TileEnvironment)).Length];
    }

    public void SetNoneLoadedData()
    {
        object type = TileType.Default;
        if (System.Enum.TryParse(typeof(TileType), tileTypeString, out type))
        {
            tileType = (TileType)type;
        }

        for (int i = 0; i < tileSpriteArray.Length; i++)
        {
            if(tileType == TileType.Creep)
            {
                tileSpriteArray[i] = Resources.Load<Sprite>("Sprite/Tile/" + ((TileEnvironment)i).ToString() + TileType.Default.ToString());
            }
            else
            {
                tileSpriteArray[i] = Resources.Load<Sprite>("Sprite/Tile/" + ((TileEnvironment)i).ToString() + tileTypeString);
            }
           
            //Debug.Log(((TileEnvironment)i).ToString() + tileTypeString);
            if (tileSpriteArray[i] == null)
            {
                tileSpriteArray[i] = Resources.Load<Sprite>("Sprite/Tile/Default");
            }
        }
        


    }
}
