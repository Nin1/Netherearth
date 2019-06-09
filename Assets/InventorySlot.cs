using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySlot {

    // The entity that owns this inventory slot
    public WorldEntity m_owner { get; private set; }
    WorldEntity m_item;

    // TODO: Restrictions so that certain items can/cannot be placed in this slot (e.g. only allow hats)
    // TODO: Allow things to listen to inventory slots (e.g. cursor, InventorySlotUI)

    public InventorySlot(WorldEntity owner)
    {
        m_owner = owner;
    }

    /** Set the item in this inventory slot. Returns false if the item cannot be placed in this slot. */
    public bool SetEntity(WorldEntity item)
    {
        if (!m_item && item)
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
