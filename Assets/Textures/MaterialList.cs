using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New MaterialList", menuName = "Material List", order = 51)]
public class MaterialList : ScriptableObject {
    
    [SerializeField]
    public List<MaterialData> data = new List<MaterialData>();
}
