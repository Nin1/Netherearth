using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** Basic-as-heck 3D tilemap */
public class CubeWorldRaw : MonoBehaviour
{
    [SerializeField]
    int mapWidth = 10;
    [SerializeField]
    int mapHeight = 10;
    [SerializeField]
    int mapDepth = 10;
    [SerializeField]
    GameObject cube;

    List<CubeData> cubes = new List<CubeData>();

    void Start()
    {
        // Generate map
        for (int x = 0; x < mapWidth; ++x)
        {
            for (int y = 0; y < mapHeight; ++y)
            {
                for (int z = 0; z < mapDepth; ++z)
                {
                    CubeData cubeData = new CubeData(CubeShape.FULL, QuarterAngle.ZERO, QuarterAngle.ZERO, QuarterAngle.ZERO);
                    cubes.Add(cubeData);
                    GameObject cubeObj = GameObject.Instantiate(cube, new Vector3(x, y, z), Quaternion.identity, transform);
                    cubeObj.GetComponent<MeshFilter>().mesh = cubeData.CreateMesh();
                }
            }
        }
    }
}
