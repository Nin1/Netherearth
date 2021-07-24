using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SingleCubeTest : MonoBehaviour
{
    [SerializeField]
    CubeShape shape = CubeShape.FULL;
    [SerializeField]
    QuarterAngle yaw = QuarterAngle.ZERO;
    [SerializeField]
    QuarterAngle pitch = QuarterAngle.ZERO;
    [SerializeField]
    QuarterAngle roll = QuarterAngle.ZERO;

    void Update()
    {
        // Single full cube with no rotation
        CubeData cube = new CubeData(shape, yaw, pitch, roll);

        MeshFilter mf = GetComponent<MeshFilter>();
        mf.mesh = cube.CreateMesh();
    }
}
