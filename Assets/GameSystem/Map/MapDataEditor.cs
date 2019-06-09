using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapData))]
public class MapDataEditor : Editor {

    struct SelectedTile
    {
        public int x;
        public int z;

        public SelectedTile(int _x, int _z)
        {
            x = _x;
            z = _z;
        }
    }

    MapData m_data;
    List<SelectedTile> m_selectedTiles = new List<SelectedTile>();
    Vector2 scrollPosition;

    public void OnEnable()
    {
        m_data = (MapData)target;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        m_data.mapName = EditorGUILayout.TextField("Map Name", m_data.mapName);
        int mapWidth = EditorGUILayout.IntField("Map width", m_data.tileData.Count);
        int mapHeight = 0;
        if (m_data.tileData.Count > 0)
        {
            mapHeight = EditorGUILayout.IntField("Map Height", m_data.tileData[0].data.Count);
        }
        UpdateMapSize(mapWidth, mapHeight);
        DrawMap(mapWidth, mapHeight);
        ShowTileEditor();

        EditorUtility.SetDirty(target);
        serializedObject.ApplyModifiedProperties();
    }

    void UpdateMapSize(int width, int height)
    {
        while (m_data.tileData.Count > width)
        {
            Debug.Log("Shrinking Map");
            m_data.tileData.RemoveAt(m_data.tileData.Count - 1);
        }
        while (m_data.tileData.Count < width)
        {
            m_data.tileData.Add(new MapData.MapTileList());
        }

        foreach(MapData.MapTileList column in m_data.tileData)
        {
            while (column.data.Count > height)
            {
                column.data.RemoveAt(column.data.Count - 1);
            }
            while (column.data.Count < height)
            {
                column.data.Add(new MapTileData());
            }
        }
    }

    void DrawMap(int width, int height)
    {
        /*
        string[] buttonLabels = new string[width * height];

        for (int z = 0; z < height; z++)
        {
            for (int x = 0; x < width; x++)
            {
                buttonLabels[x + z * width] = x + ", " + z;
            }
        }
        m_selected = GUILayout.SelectionGrid(m_selected, buttonLabels, width);
        */

        GUIStyle normal = new GUIStyle(GUIStyle.none);
        normal.fixedWidth = 16;
        normal.fixedHeight = 16;
        normal.padding = new RectOffset(1, 0, 1, 0);

        GUIStyle selected = new GUIStyle(GUIStyle.none);
        selected.fixedWidth = 16;
        selected.fixedHeight = 16;
        selected.padding = new RectOffset(2, 1, 2, 1);

        scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(400), GUILayout.Height(400));

        // We want to draw from top to bottom, so up is north
        for (int z = height - 1; z >= 0; z--)
        {
            EditorGUILayout.BeginHorizontal();
            for (int x = 0; x < width; x++)
            {
                MapTileData tile = m_data.tileData[x].data[z];
                Texture2D tex = MaterialStore.GetTexture(tile.m_floorMat);
                if(tex == null || tile.m_wallShape == MapWallShape.CLOSED_SQUARE)
                {
                    tex = Texture2D.whiteTexture;
                }

                GUIStyle style = normal;
                if(IsSelected(x, z))
                {
                    style = selected;
                }

                if(GUILayout.Button(tex, style))
                {
                    if(!(Event.current.control))
                    {
                        m_selectedTiles.Clear();
                    }
                    m_selectedTiles.Add(new SelectedTile(x, z));
                    GUI.FocusControl(null);
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        GUILayout.EndScrollView();
    }

    bool IsSelected(int x, int z)
    {
        foreach (SelectedTile tile in m_selectedTiles)
        {
            if(tile.x == x && tile.z == z)
            {
                return true;
            }
        }
        return false;
    }

    void ShowTileEditor()
    {
        if (m_selectedTiles.Count == 0) return;

        SelectedTile firstSelected = m_selectedTiles[0];
        
        if(firstSelected.x < m_data.tileData.Count && firstSelected.x >= 0)
        {
            if(firstSelected.z < m_data.tileData[0].data.Count && firstSelected.z >= 0)
            {
                MapTileData tile = m_data.tileData[firstSelected.x].data[firstSelected.z];
                int floorHeight = EditorGUILayout.IntField("Floor height", tile.m_floorHeight);
                MapFloorShape floorShape = (MapFloorShape)EditorGUILayout.EnumPopup("Floor shape", tile.m_floorShape);
                MapWallShape wallShape = (MapWallShape)EditorGUILayout.EnumPopup("Wall shape", tile.m_wallShape);
                int northMat = EditorGUILayout.IntField("North Wall Material", tile.m_northMat);
                int eastMat = EditorGUILayout.IntField("East Wall Material", tile.m_eastMat);
                int southMat = EditorGUILayout.IntField("South Wall Material", tile.m_southMat);
                int westMat = EditorGUILayout.IntField("West Wall Material", tile.m_westMat);
                int floorMat = EditorGUILayout.IntField("Floor Wall Material", tile.m_floorMat);

                foreach(SelectedTile st in m_selectedTiles)
                {
                    m_data.tileData[st.x].data[st.z].m_floorHeight = floorHeight;
                    m_data.tileData[st.x].data[st.z].m_floorShape = floorShape;
                    m_data.tileData[st.x].data[st.z].m_wallShape = wallShape;
                    m_data.tileData[st.x].data[st.z].m_northMat = northMat;
                    m_data.tileData[st.x].data[st.z].m_eastMat = eastMat;
                    m_data.tileData[st.x].data[st.z].m_southMat = southMat;
                    m_data.tileData[st.x].data[st.z].m_westMat = westMat;
                    m_data.tileData[st.x].data[st.z].m_floorMat = floorMat;
                }
            }
        }
    }
}
