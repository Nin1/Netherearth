using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct MapTileMeshes
{
    public Mesh northWall;
    public Mesh eastWall;
    public Mesh southWall;
    public Mesh westWall;
    public Mesh floor;
}

[System.Serializable]
public class MapTileData {

    /** Material Data */
    [SerializeField]
    public int m_northMat = 0;
    [SerializeField]
    public int m_eastMat = 0;
    [SerializeField]
    public int m_southMat = 0;
    [SerializeField]
    public int m_westMat = 0;
    [SerializeField]
    public int m_floorMat = 1;

    /** Physical Properties */
    [SerializeField]
    public MapFloorShape m_floorShape = MapFloorShape.FLAT;
    [SerializeField]
    public MapWallShape m_wallShape = MapWallShape.CLOSED_SQUARE;
    [SerializeField]
    public int m_floorHeight = 16;

    /** Tile constants */
    public const int SIZE = 4;
    public const int m_slopeHeight = 1;    // This could be variable later


    public bool IsDiagonal()
    {
        return m_wallShape == MapWallShape.DIAGONAL_NE
            || m_wallShape == MapWallShape.DIAGONAL_NW
            || m_wallShape == MapWallShape.DIAGONAL_SE
            || m_wallShape == MapWallShape.DIAGONAL_SW;
    }

    public bool IsDirectionOpen(CompassDirection direction)
    {
        if (m_wallShape == MapWallShape.OPEN_SQUARE) return true;
        if (m_wallShape == MapWallShape.CLOSED_SQUARE) return false;
        if (direction == CompassDirection.NORTH && (m_wallShape == MapWallShape.DIAGONAL_NE || m_wallShape == MapWallShape.DIAGONAL_NW)) return true;
        if (direction == CompassDirection.EAST && (m_wallShape == MapWallShape.DIAGONAL_NE || m_wallShape == MapWallShape.DIAGONAL_SE)) return true;
        if (direction == CompassDirection.SOUTH && (m_wallShape == MapWallShape.DIAGONAL_SE || m_wallShape == MapWallShape.DIAGONAL_SW)) return true;
        if (direction == CompassDirection.WEST && (m_wallShape == MapWallShape.DIAGONAL_NW || m_wallShape == MapWallShape.DIAGONAL_SW)) return true;
        return false;
    }

    // Get the base height of the floor in the given direction (ignores sloped edges)
    public int GetFloorHeightAtDirection(CompassDirection direction)
    {
        switch (m_floorShape)
        {
            case MapFloorShape.SLOPE_EAST:
                if (direction == CompassDirection.EAST) return m_floorHeight + m_slopeHeight;
                break;
            case MapFloorShape.SLOPE_NORTH:
                if (direction == CompassDirection.NORTH) return m_floorHeight + m_slopeHeight;
                break;
            case MapFloorShape.SLOPE_SOUTH:
                if (direction == CompassDirection.SOUTH) return m_floorHeight + m_slopeHeight;
                break;
            case MapFloorShape.SLOPE_WEST:
                if (direction == CompassDirection.WEST) return m_floorHeight + m_slopeHeight;
                break;
        }
        return m_floorHeight;
    }

    // TODO: Put in "CompassDirection" class or something?
    public CompassDirection GetOppositeDirection(CompassDirection direction)
    {
        if (direction == CompassDirection.NORTH) return CompassDirection.SOUTH;
        if (direction == CompassDirection.SOUTH) return CompassDirection.NORTH;
        if (direction == CompassDirection.EAST) return CompassDirection.WEST;
        return CompassDirection.EAST;
    }

    // Mesh:
    // Floor mesh
    // Wall meshes around floor mesh extending downwards, facing outwards
    // Ceiling mesh
    // Wall meshes around ceiling extending upwards, facing outwards
    //
    // Only create wall meshes if that wall is visible

    public MapTileMeshes GenerateMesh(MapTileData north, MapTileData east, MapTileData south, MapTileData west)
    {
        MapTileMeshes meshes;
        // Make floor
        meshes.floor = CreateFloorMesh();

        // TODO: Account for diagonals

        // Make north wall
        meshes.northWall = CreateAlignedSingleWallMesh(north, CompassDirection.NORTH);
        // Make east wall
        meshes.eastWall = CreateAlignedSingleWallMesh(east, CompassDirection.EAST);
        meshes.eastWall = RotateMeshAroundPoint(meshes.eastWall, 90, new Vector3(2, 0, 2), Vector3.up);
        // Make south wall
        meshes.southWall = CreateAlignedSingleWallMesh(south, CompassDirection.SOUTH);
        meshes.southWall = RotateMeshAroundPoint(meshes.southWall, 180, new Vector3(2, 0, 2), Vector3.up);
        // Make west wall
        meshes.westWall = CreateAlignedSingleWallMesh(west, CompassDirection.WEST);
        meshes.westWall = RotateMeshAroundPoint(meshes.westWall, 270, new Vector3(2, 0, 2), Vector3.up);

        meshes.floor.RecalculateBounds();
        meshes.floor.RecalculateNormals();
        meshes.floor.RecalculateTangents();
        meshes.northWall.RecalculateBounds();
        meshes.northWall.RecalculateNormals();
        meshes.northWall.RecalculateTangents();
        meshes.eastWall.RecalculateBounds();
        meshes.eastWall.RecalculateNormals();
        meshes.eastWall.RecalculateTangents();
        meshes.southWall.RecalculateBounds();
        meshes.southWall.RecalculateNormals();
        meshes.southWall.RecalculateTangents();
        meshes.westWall.RecalculateBounds();
        meshes.westWall.RecalculateNormals();
        meshes.westWall.RecalculateTangents();

        return meshes;
    }

    // Create a mesh for this tile, using data from the surrounding tiles 
    public Mesh CreateMesh(MapTileData north, MapTileData east, MapTileData south, MapTileData west)
    {
        Mesh floor = CreateFloorMesh();
        Mesh wall = new Mesh();
        if (IsDiagonal())
        {
            // Make diagonal walls eep
        }
        else
        {
            wall = CreateAlignedWallMesh(north, east, south, west);
        }


        CombineInstance[] ci = new CombineInstance[2];
        ci[0].mesh = floor;
        ci[1].mesh = wall;
        ci[0].transform = Matrix4x4.identity;
        ci[1].transform = Matrix4x4.identity;

        Mesh mesh = new Mesh();
        mesh.CombineMeshes(ci);

        return mesh;
    }

    // Create the floor mesh for this tile
    Mesh CreateFloorMesh()
    {
        List<Vector3> vertices = new List<Vector3>();
        List<Vector2> uv = new List<Vector2>();
        List<int> triangles = new List<int>();

        int floorNE = m_floorHeight;
        int floorNW = m_floorHeight;
        int floorSE = m_floorHeight;
        int floorSW = m_floorHeight;

        switch (m_floorShape)
        {
            case MapFloorShape.SLOPE_NORTH:
                floorNE++;
                floorNW++;
                break;
            case MapFloorShape.SLOPE_EAST:
                floorNE++;
                floorSE++;
                break;
            case MapFloorShape.SLOPE_SOUTH:
                floorSE++;
                floorSW++;
                break;
            case MapFloorShape.SLOPE_WEST:
                floorNW++;
                floorSW++;
                break;
            default:
                break;
        }

        switch (m_wallShape)
        {
            // For diagonal shapes, the floor is flat and we ignore one corner
            case MapWallShape.DIAGONAL_NE:
                vertices.Add(new Vector3(0, floorNE, 4));
                vertices.Add(new Vector3(4, floorNW, 4));
                vertices.Add(new Vector3(4, floorSW, 0));
                uv.Add(new Vector2(0, 1));
                uv.Add(new Vector2(1, 1));
                uv.Add(new Vector2(1, 0));
                triangles.Add(0);
                triangles.Add(1);
                triangles.Add(2);
                break;
            case MapWallShape.DIAGONAL_NW:
                vertices.Add(new Vector3(0, floorSE, 0));
                vertices.Add(new Vector3(0, floorNE, 4));
                vertices.Add(new Vector3(4, floorNW, 4));
                uv.Add(new Vector2(0, 0));
                uv.Add(new Vector2(0, 1));
                uv.Add(new Vector2(1, 1));
                triangles.Add(0);
                triangles.Add(1);
                triangles.Add(2);
                break;
            case MapWallShape.DIAGONAL_SE:
                vertices.Add(new Vector3(0, floorSE, 0));
                vertices.Add(new Vector3(4, floorNW, 4));
                vertices.Add(new Vector3(4, floorSW, 0));
                uv.Add(new Vector2(0, 0));
                uv.Add(new Vector2(1, 1));
                uv.Add(new Vector2(1, 0));
                triangles.Add(0);
                triangles.Add(1);
                triangles.Add(2);
                break;
            case MapWallShape.DIAGONAL_SW:
                vertices.Add(new Vector3(0, floorSE, 0));
                vertices.Add(new Vector3(0, floorNE, 4));
                vertices.Add(new Vector3(4, floorSW, 0));
                uv.Add(new Vector2(0, 0));
                uv.Add(new Vector2(0, 1));
                uv.Add(new Vector2(1, 0));
                triangles.Add(0);
                triangles.Add(1);
                triangles.Add(2);
                break;
            case MapWallShape.OPEN_SQUARE:
                vertices.Add(new Vector3(0, floorSW, 0));
                vertices.Add(new Vector3(0, floorNW, 4));
                vertices.Add(new Vector3(4, floorNE, 4));
                vertices.Add(new Vector3(4, floorSE, 0));
                uv.Add(new Vector2(0, 0));
                uv.Add(new Vector2(0, 1));
                uv.Add(new Vector2(1, 1));
                uv.Add(new Vector2(1, 0));
                triangles.Add(0);
                triangles.Add(1);
                triangles.Add(2);
                triangles.Add(0);
                triangles.Add(2);
                triangles.Add(3);
                break;
        }

        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.uv = uv.ToArray();
        mesh.triangles = triangles.ToArray();

        return mesh;
    }

    // Create the wall mesh for this tile
    Mesh CreateAlignedWallMesh(MapTileData north, MapTileData east, MapTileData south, MapTileData west)
    {
        // SW is 0,0,0
        // NE is 4,0,4

        // Make north wall
        Mesh northMesh = CreateAlignedSingleWallMesh(north, CompassDirection.NORTH);
        // Make east wall
        Mesh eastMesh = CreateAlignedSingleWallMesh(east, CompassDirection.EAST);
        eastMesh = RotateMeshAroundPoint(eastMesh, 90, new Vector3(2, 0, 2), Vector3.up);
        // Make south wall
        Mesh southMesh = CreateAlignedSingleWallMesh(south, CompassDirection.SOUTH);
        southMesh = RotateMeshAroundPoint(southMesh, 180, new Vector3(2, 0, 2), Vector3.up);
        // Make west wall
        Mesh westMesh = CreateAlignedSingleWallMesh(west, CompassDirection.WEST);
        westMesh = RotateMeshAroundPoint(westMesh, 270, new Vector3(2, 0, 2), Vector3.up);

        // Combine all meshes
        CombineInstance[] ci = new CombineInstance[4];
        ci[0].mesh = northMesh;
        ci[1].mesh = eastMesh;
        ci[2].mesh = southMesh;
        ci[3].mesh = westMesh;
        ci[0].transform = Matrix4x4.identity;
        ci[1].transform = Matrix4x4.identity;
        ci[2].transform = Matrix4x4.identity;
        ci[3].transform = Matrix4x4.identity;

        Mesh mesh = new Mesh();
        mesh.CombineMeshes(ci);

        foreach (Vector3 v in mesh.vertices)
        {
            Debug.Log(v);
        }
        return mesh;
    }

    // Create wall mesh from the floor to the given height, as if it were the north wall (facing outwards
    Mesh CreateAlignedSingleWallMesh(MapTileData neighbour, CompassDirection direction)
    {
        int wallBottom = 0;
        CompassDirection neighbourDirection = GetOppositeDirection(direction);
        if (neighbour.IsDirectionOpen(neighbourDirection))
        {
            wallBottom = neighbour.GetFloorHeightAtDirection(neighbourDirection);
        }
        else
        {
            return new Mesh();
        }
        int wallTop = GetFloorHeightAtDirection(direction);
        // If this tile's floor is below the neighbour's, we have no wall to make (TODO: Unless slopes)
        if (wallTop < wallBottom) return new Mesh();

        List<Vector3> vertices = new List<Vector3>();
        List<Vector2> uv = new List<Vector2>();
        List<int> triangles = new List<int>();
        int index = 0;
        int currentBaseHeight = wallBottom;
        float currentUvOffset = (currentBaseHeight % 4) / 4.0f;

        // Make quads from current height to the next multiple of 4
        while (currentBaseHeight < wallTop)
        {
            // Create square quads going upwards in 4-unit increments (starting from 0)
            int nextBaseHeight = ((currentBaseHeight / 4) * 4) + 4;
            nextBaseHeight = Mathf.Min(wallTop, nextBaseHeight);
            float nextUvOffset = (nextBaseHeight % 4) / 4.0f;

            // Make sure to use the correct y UV
            if (nextUvOffset == 0.0f) nextUvOffset = 1.0f;

            vertices.Add(new Vector3(4, currentBaseHeight, 4));
            vertices.Add(new Vector3(4, nextBaseHeight, 4));
            vertices.Add(new Vector3(0, nextBaseHeight, 4));
            vertices.Add(new Vector3(0, currentBaseHeight, 4));
            uv.Add(new Vector2(1, currentUvOffset));
            uv.Add(new Vector2(1, nextUvOffset));
            uv.Add(new Vector2(0, nextUvOffset));
            uv.Add(new Vector2(0, currentUvOffset));
            triangles.Add(index);
            triangles.Add(index + 1);
            triangles.Add(index + 2);
            triangles.Add(index);
            triangles.Add(index + 2);
            triangles.Add(index + 3);

            // Set up for next shape
            currentBaseHeight = nextBaseHeight;
            currentUvOffset = nextUvOffset;
            if (currentUvOffset == 1.0f) currentUvOffset = 0.0f;

            index += 4;
        }

        // Create triangle for our sloped floor
        if ((m_floorShape == MapFloorShape.SLOPE_EAST && direction == CompassDirection.NORTH) ||
             (m_floorShape == MapFloorShape.SLOPE_WEST && direction == CompassDirection.SOUTH) ||
             (m_floorShape == MapFloorShape.SLOPE_SOUTH && direction == CompassDirection.EAST) ||
             (m_floorShape == MapFloorShape.SLOPE_NORTH && direction == CompassDirection.WEST))
        {
            vertices.Add(new Vector3(0, currentBaseHeight, 4));
            vertices.Add(new Vector3(4, currentBaseHeight, 4));
            vertices.Add(new Vector3(4, currentBaseHeight + 1, 4));
            uv.Add(new Vector2(0, currentUvOffset));
            uv.Add(new Vector2(1, currentUvOffset));
            currentUvOffset += 0.25f;
            uv.Add(new Vector2(1, currentUvOffset));
            triangles.Add(index++);
            triangles.Add(index++);
            triangles.Add(index++);
            currentBaseHeight++;
        }
        else if ((m_floorShape == MapFloorShape.SLOPE_EAST && direction == CompassDirection.SOUTH) ||
                (m_floorShape == MapFloorShape.SLOPE_WEST && direction == CompassDirection.NORTH) ||
                (m_floorShape == MapFloorShape.SLOPE_SOUTH && direction == CompassDirection.WEST) ||
                (m_floorShape == MapFloorShape.SLOPE_NORTH && direction == CompassDirection.EAST))
        {
            vertices.Add(new Vector3(0, currentBaseHeight, 4));
            vertices.Add(new Vector3(4, currentBaseHeight, 4));
            vertices.Add(new Vector3(0, currentBaseHeight + 1, 4));
            uv.Add(new Vector2(0, currentUvOffset));
            uv.Add(new Vector2(1, currentUvOffset));
            currentUvOffset += 0.25f;
            uv.Add(new Vector2(0, currentUvOffset));
            triangles.Add(index++);
            triangles.Add(index++);
            triangles.Add(index++);
            currentBaseHeight++;
        }

        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.uv = uv.ToArray();
        mesh.triangles = triangles.ToArray();

        return mesh;
    }

    Mesh RotateMeshAroundPoint(Mesh mesh, float degrees, Vector3 point, Vector3 axis)
    {
        Vector3[] newVertices = new Vector3[mesh.vertexCount];
        Quaternion rot = Quaternion.AngleAxis(degrees, axis);
        int i = 0;
        foreach (Vector3 vertex in mesh.vertices)
        {
            newVertices[i] = vertex - point;
            newVertices[i] = rot * newVertices[i];
            newVertices[i] = point + newVertices[i];
            i++;
        }
        mesh.vertices = newVertices;
        return mesh;
    }
}
