using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * MapTileObject
 * Game world object to represent a map tile and control its meshes
 */
[ExecuteInEditMode]
public class MapTileObject : MonoBehaviour {

    public MapTilePiece m_northWall;
    public MapTilePiece m_eastWall;
    public MapTilePiece m_southWall;
    public MapTilePiece m_westWall;
    public MapTilePiece m_floor;

    MapTile m_tile;

    // Returns true if anything exists in the tile
    public void SetMapTile(MapTileData tileData, MapTileMeshes meshes)
    {
        m_northWall.SetMesh(meshes.northWall, tileData.m_northMat);
        m_eastWall.SetMesh(meshes.eastWall, tileData.m_eastMat);
        m_southWall.SetMesh(meshes.southWall, tileData.m_southMat);
        m_westWall.SetMesh(meshes.westWall, tileData.m_westMat);
        m_floor.SetMesh(meshes.floor, tileData.m_floorMat);
    }
}
