using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Map", menuName = "Map Data", order = 51)]
public class MapData : ScriptableObject {
    
    [System.Serializable]
    public class MapTileList { public List<MapTileData> data = new List<MapTileData>(); }

    [SerializeField]
    public List<MapTileList> tileData = new List<MapTileList>();
    [SerializeField]
    public string mapName = "New Map";

    private static MapTileData dummyTile = new MapTileData();

    public int GetWidth()
    {
        return tileData.Count;
    }

    public int GetLength()
    {
        if(tileData.Count > 0)
        {
            return tileData[0].data.Count;
        }
        return 0;
    }

    public MapTileData GetTile(int x, int z)
    {
        if (x >= 0 && x < GetWidth() && z >= 0 && z < GetLength())
        {
            return tileData[x].data[z];
        }
        return dummyTile;
    }
}
