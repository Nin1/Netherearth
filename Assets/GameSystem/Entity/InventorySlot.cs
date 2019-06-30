using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySlot {

    // The entity that owns this inventory slot
    public WorldEntity m_owner { get; private set; }
    WorldEntity m_item;
    // Mask defining what kind of things may be "equipped" to this slot
    EquipMask m_equipMask;
    
    // TODO: Allow things to listen to inventory slots (e.g. cursor, InventorySlotUI)

    public InventorySlot(WorldEntity owner)
    {
        m_owner = owner;
        m_equipMask = EquipMask.EVERYTHING;
    }

    public InventorySlot(WorldEntity owner, EquipMask equipMask)
    {
        m_owner = owner;
        m_equipMask = equipMask;
    }

    /** Set the item in this inventory slot. Returns false if the item cannot be placed in this slot. */
    public bool SetEntity(WorldEntity item)
    {
        if (CanPlaceItem(item))
        {
            m_item = item;
            // TODO: Replace SetActive with WorldEntity::HideFromWorld()
            m_item.gameObject.SetActive(false);
            // Move item to physically be "inside" its owner
            m_item.transform.parent = m_owner.transform;
            m_item.transform.localPosition = new Vector3(0, 0, 0);
            return true;
        }
        return false;
    }

    /** Return true if we can place this item in this slot */
    bool CanPlaceItem(WorldEntity item)
    {
        // Return true if:
        return !m_item  // There isn't already an item in this slot 
            && item     // The given item is valid
            && (item.m_data.equipMask & m_equipMask) > 0;   // The given item fits our EquipMask
    }

    public WorldEntity GetEntity()
    {
        return m_item;
    }

    public bool IsEmpty()
    {
        return m_item == null;
    }

    public void ClearSlot()
    {
        m_item = null;
    }
}
