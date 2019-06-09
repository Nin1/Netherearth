using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CompassDirection
{
    NORTH,
    EAST,
    SOUTH,
    WEST
}

/**
 * MapTileWallShape
 * Describes the x/z shape of a map tile
 */
public enum MapWallShape
{
    OPEN_SQUARE,    // Open space
    CLOSED_SQUARE,  // Closed space - surrounding tiles will have walls
    DIAGONAL_NW,    // NW corner open
    DIAGONAL_NE,    // NE corner open
    DIAGONAL_SW,    // SW corner open
    DIAGONAL_SE,    // SE corner open
}

public enum MapFloorShape
{
    FLAT,
    SLOPE_NORTH,    // Slope from floor height up towards north
    SLOPE_EAST,
    SLOPE_WEST,
    SLOPE_SOUTH,
}

/**
 * MapTile
 * Contains information for a single map tile,
 * which is 4 units by 16 units by 4 units
 */
public class MapTile
{
    // IDs for each material (From the MaterialList in MaterialStore)
    public int m_northMat = 0;
    public int m_eastMat = 0;
    public int m_southMat = 0;
    public int m_westMat = 0;
    public int m_floorMat = 1;
    
    public MapFloorShape m_floorShape = MapFloorShape.FLAT;
    public MapWallShape m_wallShape = MapWallShape.CLOSED_SQUARE;
    public uint m_floorHeight = 16;
    // Properties (is water, lava, etc.)

    // The width and length of a map tile in world units
    public const uint SIZE = 4;
    public const uint m_slopeHeight = 1;    // This could be variable later

    // Meshes for each part of the tile, populated by GenerateMesh()
    public Mesh m_northMesh { get; private set; }
    public Mesh m_eastMesh { get; private set; }
    public Mesh m_southMesh { get; private set; }
    public Mesh m_westMesh { get; private set; }
    public Mesh m_floorMesh { get; private set; }
    
}
