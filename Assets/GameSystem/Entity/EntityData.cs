using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** Attributes that can apply to all entities (Yes, a door *may* be equippable) */
[CreateAssetMenu(fileName = "EntityData", menuName = "EntityData", order = 1)]
public class EntityData : ScriptableObject {
    public string examineName = "an entity";
    
    // Something here for world appearance and interface appearance
    public Sprite sprite;

    // Where can this entity be equipped?
    [EnumFlags]
    public EquipMask equipMask = EquipMask.HELD_ITEM;

    public float weightKg = 1.0f;

}
