using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New MaterialData", menuName = "Material Data", order = 51)]
public class MaterialData : ScriptableObject {

    [SerializeField]
    public Texture2D texture;
    [SerializeField]
    public Shader shader;
}
