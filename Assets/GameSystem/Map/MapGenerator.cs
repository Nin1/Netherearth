using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: Find a better way to load a map at runtime
[ExecuteInEditMode]
public class MapGenerator : MonoBehaviour {


    [SerializeField]
    GameObject m_mapTileObject;
    [SerializeField]
    GameObject m_ceilingObject;
    [SerializeField]
    MapData m_mapData;

    // Use this for initialization
    void Start () {
        // Remove all children if any
        for(int i = transform.childCount - 1; i  >= 0; i--)
        {
            Transform child = transform.GetChild(i);
            if (child.CompareTag("Terrain"))
            {
                DestroyImmediate(transform.GetChild(i).gameObject);
            }
        }

        int minX = -1;
        int maxX = -1;
        int minZ = -1;
        int maxZ = -1;

        // Build each map tile's object
		for(int x = 0; x < m_mapData.GetWidth(); x++)
        {
            for(int z = 0; z < m_mapData.GetLength(); z++)
            {
                MapTileData tile = m_mapData.GetTile(x, z);
                MapTileData north = m_mapData.GetTile(x, z + 1);
                MapTileData east = m_mapData.GetTile(x + 1, z);
                MapTileData south = m_mapData.GetTile(x, z - 1);
                MapTileData west = m_mapData.GetTile(x - 1, z);

                MapTileMeshes meshes = tile.GenerateMesh(north, east, south, west);
                if (meshes.floor.vertices.GetLength(0) == 0 &&
                    meshes.northWall.vertices.GetLength(0) == 0 &&
                    meshes.eastWall.vertices.GetLength(0) == 0 &&
                    meshes.southWall.vertices.GetLength(0) == 0 &&
                    meshes.westWall.vertices.GetLength(0) == 0)
                {
                    // No meshes generated
                    continue;
                }

                GameObject mapTile = Instantiate(m_mapTileObject, transform);
                mapTile.transform.localPosition = new Vector3(x * MapTile.SIZE, 0, z * MapTile.SIZE);
                MapTileObject mto = mapTile.GetComponent<MapTileObject>();
                mto.SetMapTile(tile, meshes);

                if(minX == -1)
                {
                    minX = x;
                    maxX = x;
                    minZ = z;
                    maxZ = z;
                }
                else
                {
                    minX = Mathf.Min(x, minX);
                    maxX = Mathf.Max(x, maxX);
                    minZ = Mathf.Min(z, minZ);
                    maxZ = Mathf.Max(z, maxZ);
                }
            }
        }

        GenerateCeiling(minX, maxX, minZ, maxZ);
	}

    void GenerateCeiling(int startX, int endX, int startZ, int endZ)
    {
        List<Vector3> vertices = new List<Vector3>();
        List<Vector2> uv = new List<Vector2>();
        List<int> triangles = new List<int>();

        int index = 0;

        for (int x = startX; x <= endX; x++)
        {
            for(int z = startZ; z <= endZ; z++)
            {
                vertices.Add(new Vector3(x * 4, 16, z * 4));
                vertices.Add(new Vector3(x * 4, 16, (z + 1) * 4));
                vertices.Add(new Vector3((x + 1) * 4, 16, (z + 1) * 4));
                vertices.Add(new Vector3((x + 1) * 4, 16, z * 4));
                uv.Add(new Vector2(0, 0));
                uv.Add(new Vector2(0, 1));
                uv.Add(new Vector2(1, 1));
                uv.Add(new Vector2(1, 0));
                triangles.Add(index);
                triangles.Add(index + 2);
                triangles.Add(index + 1);
                triangles.Add(index);
                triangles.Add(index + 3);
                triangles.Add(index + 2);
                index += 4;
            }
        }

        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.uv = uv.ToArray();
        mesh.triangles = triangles.ToArray();

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();

        GameObject ceiling = Instantiate(m_ceilingObject, transform);
        ceiling.GetComponent<MeshFilter>().mesh = mesh;
        ceiling.GetComponent<MeshCollider>().sharedMesh = mesh;
        ceiling.GetComponent<MeshRenderer>().material = MaterialStore.GetMaterial(2);
    }

}
