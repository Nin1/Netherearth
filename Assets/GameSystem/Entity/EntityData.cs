using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** Attributes that can apply to all entities (Yes, a door *may* be equippable) */
[CreateAssetMenu(fileName = "EntityData", menuName = "EntityData", order = 1)]
public class EntityData : ScriptableObject {
    public string examineName = "an entity";
    
    // Something here for world appearance and interface appearance
    public Sprite sprite;

    bool isEquippable = false;
    float weightKg = 1.0f;

}
