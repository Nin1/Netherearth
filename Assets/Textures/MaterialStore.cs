using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public struct MatProperties
{
    public string texture;
    public string shader;
}

[ExecuteInEditMode]
public class MaterialStore : MonoBehaviour {

    public static MaterialStore instance = null;

    [SerializeField]
    MaterialList m_materialList;
    Dictionary<int, Material> m_mats = new Dictionary<int, Material>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    Material GetMaterialInner(int id)
    {
        // Have we already made the material?
        foreach(KeyValuePair<int, Material> kvp in m_mats)
        {
            if(kvp.Key == id)
            {
                return kvp.Value;
            }
        }

        // Make the material
        return CreateMaterial(id);
    }

    // Create the material, cache it, and return it
    Material CreateMaterial(int id)
    {
        if(id >= m_materialList.data.Count)
        {
            Debug.LogError("Invalid Material ID: " + id);
            return null;
        }

        MaterialData data = m_materialList.data[id];
        Material newMat = new Material(data.shader);
        newMat.mainTexture = data.texture;
        m_mats.Add(id, newMat);
        return newMat;
    }

    public static Material GetMaterial(int id)
    {
        if (instance != null)
        {
            return instance.GetMaterialInner(id);
        }
        return null;
    }

    public static Texture2D GetTexture(int id)
    {
        if (instance != null)
        {
            Material mat = instance.GetMaterialInner(id);
            if (mat != null)
            {
                return (Texture2D)mat.mainTexture;
            }
        }
        return null;
    }
}
