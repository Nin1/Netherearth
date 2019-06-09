using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** 
 * TiledMap
 * An area made out of tiles.
 * Maps in UW are 64x64 tiles
 */
public class TiledMap {
    MapTile[,] m_tiles = new MapTile[5,5];
    MapTile m_dummyTile = new MapTile();

    public TiledMap()
    {
        for(int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                m_tiles[i, j] = new MapTile();
            }
        }

        m_dummyTile.m_wallShape = MapWallShape.CLOSED_SQUARE;

        m_tiles[1, 3].m_wallShape = MapWallShape.OPEN_SQUARE;   m_tiles[2, 3].m_wallShape = MapWallShape.OPEN_SQUARE;   m_tiles[3, 3].m_wallShape = MapWallShape.OPEN_SQUARE;
        m_tiles[1, 3].m_floorShape = MapFloorShape.SLOPE_EAST;  m_tiles[2, 3].m_floorShape = MapFloorShape.SLOPE_EAST;  m_tiles[3, 3].m_floorShape = MapFloorShape.SLOPE_EAST;
        m_tiles[1, 3].m_floorHeight = 1;                        m_tiles[2, 3].m_floorHeight = 2;                        m_tiles[3, 3].m_floorHeight = 3;

        m_tiles[1, 2].m_wallShape = MapWallShape.OPEN_SQUARE;   m_tiles[2, 2].m_wallShape = MapWallShape.OPEN_SQUARE;   m_tiles[3, 2].m_wallShape = MapWallShape.OPEN_SQUARE;   
        m_tiles[1, 2].m_floorShape = MapFloorShape.SLOPE_WEST;  m_tiles[2, 2].m_floorShape = MapFloorShape.FLAT;        m_tiles[3, 2].m_floorShape = MapFloorShape.SLOPE_SOUTH; 
        m_tiles[1, 2].m_floorHeight = 0;                        m_tiles[2, 2].m_floorHeight = 0;                        m_tiles[3, 2].m_floorHeight = 4;                        
        
        m_tiles[1, 1].m_wallShape = MapWallShape.OPEN_SQUARE;   m_tiles[2, 1].m_wallShape = MapWallShape.OPEN_SQUARE;   m_tiles[3, 1].m_wallShape = MapWallShape.OPEN_SQUARE;
        m_tiles[1, 1].m_floorShape = MapFloorShape.SLOPE_WEST;  m_tiles[2, 1].m_floorShape = MapFloorShape.SLOPE_WEST;  m_tiles[3, 1].m_floorShape = MapFloorShape.SLOPE_SOUTH;
        m_tiles[1, 1].m_floorHeight = 7;                        m_tiles[2, 1].m_floorHeight = 6;                        m_tiles[3, 1].m_floorHeight = 5;
    }

    public MapTile GetTile(uint x, uint z)
    {
        if (x < m_tiles.GetLowerBound(0) || x > m_tiles.GetUpperBound(0) ||
            z < m_tiles.GetLowerBound(1) || z > m_tiles.GetUpperBound(1))
        {
            return m_dummyTile;
        }
        return m_tiles[x, z];
    }

    public int GetWidth()
    {
        return m_tiles.GetUpperBound(0) + 1;
    }

    public int GetLength()
    {
        return m_tiles.GetUpperBound(1) + 1;
    }

    /*
    public Mesh GetFullMesh()
    {
        List<CombineInstance> meshes = new List<CombineInstance>();
        for(int x = 0; x < 5; x++)
        {
            for(int z = 0; z < 5; z++)
            {
                MapTile north;
                MapTile east;
                MapTile south;
                MapTile west;

                if (x == 0)
                {
                    west = m_dummyTile;
                }
                else
                {
                    west = m_tiles[x - 1, z];
                }
                if (x == 4)
                {
                    east = m_dummyTile;
                }
                else
                {
                    east = m_tiles[x + 1, z];
                }

                if (z == 0)
                {
                    south = m_dummyTile;
                }
                else
                {
                    south = m_tiles[x, z - 1];
                }
                if (z == 4)
                {
                    north = m_dummyTile;
                }
                else
                {
                    north = m_tiles[x, z + 1];
                }


                Mesh mesh = m_tiles[x, z].CreateMesh(north, east, south, west);

                Vector3[] verts = new Vector3[mesh.vertexCount];
                int i = 0;
                foreach(Vector3 vert in mesh.vertices)
                {
                    verts[i] = vert + new Vector3(x * 4, 0, z * 4);
                    i++;
                }
                mesh.vertices = verts;

                CombineInstance ci = new CombineInstance();
                ci.mesh = mesh;
                ci.transform = Matrix4x4.identity;
                meshes.Add(ci);
            }
        }

        Mesh finalMesh = new Mesh();
        finalMesh.CombineMeshes(meshes.ToArray());
        finalMesh.RecalculateBounds();
        finalMesh.RecalculateNormals();
        finalMesh.RecalculateTangents();
        return finalMesh;
    }
    */
}
