using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * A piece of a map tile's geometry (wall or floor)
 * Handles setting the mesh for this piece
 */
[ExecuteInEditMode]
public class MapTilePiece : MonoBehaviour {

    MeshFilter m_meshFilter;
    MeshCollider m_meshCollider;
    MeshRenderer m_meshRenderer;
    
	void Awake () {
        m_meshFilter = GetComponent<MeshFilter>();
        m_meshCollider = GetComponent<MeshCollider>();
        m_meshRenderer = GetComponent<MeshRenderer>();
	}

    public void SetMesh(Mesh mesh, int mat)
    {
        m_meshFilter.mesh = mesh;
        m_meshCollider.sharedMesh = mesh;
        m_meshRenderer.material = MaterialStore.GetMaterial(mat);
    }
}
