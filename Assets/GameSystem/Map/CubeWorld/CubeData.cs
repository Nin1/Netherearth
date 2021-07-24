using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CubeShape
{
    EMPTY,
    FULL,
    SLOPE,
    CORNER
}

public enum QuarterAngle
{
    ZERO,
    NINETY,
    ONEEIGHTY,
    TWOSEVENTY
}

/** Shape data for a single world cube */
public class CubeData
{
    int data;

    /** Shape: 0 = empty, 1 = full, 2 = slope, 3 = corner
        Yaw/Pitch/Roll: 0 = 0deg, 1 = 90deg, 2 = 180deg, 3 = 270deg
    */
    public CubeData(CubeShape shape, QuarterAngle yaw, QuarterAngle pitch, QuarterAngle roll)
    {
        data = 0;
        data |= ((int)shape & 0x3);
        data |= (((int)yaw & 0x3) << 2);
        data |= (((int)pitch & 0x3) << 4);
        data |= (((int)roll & 0x3) << 6);
    }

    /*
     * Shapes (2 bits):
     * 1 - Full (Default: Front faces -z
     * 2 - Slope (Default: from -z upwards to +z)
     * 3 - Corner (Default: Corner +x, +y, -z missing)
     */
    public int GetShape()
    {
        return data & 0x3;
    }

    /*
     * Rotation (3 bits):
     * Bits 0-1 : Yaw
     * Bits 2-3 : Pitch
     * Bits 4-5 : Roll
     */

    /*
     * BlockType
     */

    /*
     * Block data?
     */

    public Mesh CreateMesh()
    {
        Mesh mesh;

        // data & 0x3 is the shape
        switch(data & 0x3)
        {
            case 1:
                mesh = CreateFullMesh();
                break;
            case 2:
                mesh = CreateSlopeMesh();
                break;
            case 3:
                mesh = CreateCornerMesh();
                break;
            default:
                return new Mesh();
        }

        float yaw = 90.0f * ((data >> 2) & 0x3);
        float pitch = 90.0f * ((data >> 4) & 0x3);
        float roll = 90.0f * ((data >> 6) & 0x3);

        RotateMesh(mesh, yaw, pitch, roll);

        mesh.RecalculateBounds();
        mesh.RecalculateTangents();
        mesh.RecalculateNormals();
        return mesh;
    }

    // Creates a full cube, front face towards -z
    Mesh CreateFullMesh()
    {
        List<Vector3> vertices = new List<Vector3>();
        List<Vector2> uv = new List<Vector2>();
        List<int> triangles = new List<int>();

        // Create 6 walls
        // 8 Vertices
        vertices.Add(new Vector3(0, 0, 0)); // 0
        vertices.Add(new Vector3(0, 0, 1)); // 1
        vertices.Add(new Vector3(1, 0, 0)); // 2
        vertices.Add(new Vector3(1, 0, 1)); // 3
        vertices.Add(new Vector3(0, 1, 0)); // 4
        vertices.Add(new Vector3(0, 1, 1)); // 5
        vertices.Add(new Vector3(1, 1, 0)); // 6
        vertices.Add(new Vector3(1, 1, 1)); // 7
        // 12 triangles
        triangles.Add(2); triangles.Add(1); triangles.Add(0);   // Bottom*
        triangles.Add(2); triangles.Add(3); triangles.Add(1);
        triangles.Add(4); triangles.Add(5); triangles.Add(7);   // Top*
        triangles.Add(7); triangles.Add(6); triangles.Add(4);
        triangles.Add(0); triangles.Add(1); triangles.Add(4);   // Left*
        triangles.Add(1); triangles.Add(5); triangles.Add(4);
        triangles.Add(6); triangles.Add(3); triangles.Add(2);   // Right*
        triangles.Add(6); triangles.Add(7); triangles.Add(3);
        triangles.Add(0); triangles.Add(4); triangles.Add(2);   // Front*
        triangles.Add(4); triangles.Add(6); triangles.Add(2);
        triangles.Add(1); triangles.Add(3); triangles.Add(5);   // Back*
        triangles.Add(3); triangles.Add(7); triangles.Add(5);

        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.uv = uv.ToArray();
        mesh.triangles = triangles.ToArray();
        return mesh;
    }

    // Creates a slope, missing the faces at -z, y
    Mesh CreateSlopeMesh()
    {
        List<Vector3> vertices = new List<Vector3>();
        List<Vector2> uv = new List<Vector2>();
        List<int> triangles = new List<int>();

        // Create 2 full walls
        // 6 Vertices
        vertices.Add(new Vector3(0, 0, 0)); // 0
        vertices.Add(new Vector3(0, 0, 1)); // 1
        vertices.Add(new Vector3(1, 0, 0)); // 2
        vertices.Add(new Vector3(1, 0, 1)); // 3
        vertices.Add(new Vector3(0, 1, 1)); // 4
        vertices.Add(new Vector3(1, 1, 1)); // 5
        // 4 Triangles
        triangles.Add(2); triangles.Add(1); triangles.Add(0);   // Bottom*
        triangles.Add(2); triangles.Add(3); triangles.Add(1);
        triangles.Add(1); triangles.Add(3); triangles.Add(4);   // Back*
        triangles.Add(3); triangles.Add(5); triangles.Add(4);
        // Create 2 triangular walls
        triangles.Add(1); triangles.Add(4); triangles.Add(0);   // Left*
        triangles.Add(2); triangles.Add(5); triangles.Add(3);   // Right*
        // Create 1 diagonal wall/floor
        triangles.Add(0); triangles.Add(4); triangles.Add(5);
        triangles.Add(0); triangles.Add(5); triangles.Add(2);

        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.uv = uv.ToArray();
        mesh.triangles = triangles.ToArray();
        return mesh;
    }

    // Creates a corner, missing the faces at -z, +y, +x
    Mesh CreateCornerMesh()
    {
        List<Vector3> vertices = new List<Vector3>();
        List<Vector2> uv = new List<Vector2>();
        List<int> triangles = new List<int>();

        // Create 4 triangular walls
        // 4 Vertices
        vertices.Add(new Vector3(0, 0, 0)); // 0
        vertices.Add(new Vector3(0, 0, 1)); // 1
        vertices.Add(new Vector3(1, 0, 1)); // 2
        vertices.Add(new Vector3(0, 1, 1)); // 3
        // 4 Triangles
        triangles.Add(2); triangles.Add(1); triangles.Add(0);   // Bottom
        triangles.Add(2); triangles.Add(3); triangles.Add(1);   // Back
        triangles.Add(0); triangles.Add(1); triangles.Add(3);   // Left
        triangles.Add(0); triangles.Add(3); triangles.Add(2);   // Across

        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.uv = uv.ToArray();
        mesh.triangles = triangles.ToArray();
        return mesh;
    }

    Mesh RotateMesh(Mesh mesh, float yaw, float pitch, float roll)
    {
        Vector3[] newVertices = new Vector3[mesh.vertexCount];
        Vector3 center = new Vector3(0.5f, 0.5f, 0.5f);
        Quaternion rot = Quaternion.Euler(pitch, yaw, roll);
        int i = 0;
        foreach (Vector3 vertex in mesh.vertices)
        {
            newVertices[i] = vertex - center;
            newVertices[i] = rot * newVertices[i];
            newVertices[i] = center + newVertices[i];
            i++;
        }
        mesh.vertices = newVertices;
        return mesh;
    }
}
