using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerInventorySlot
{
    HELD_ITEM,
    BAG_SLOT_1,
    BAG_SLOT_2,
    BAG_SLOT_3,
    BAG_SLOT_4,
    HEAD,
    BODY,
    LEGS,
    FEET,
    HANDS,
    MAIN_HAND,
    OFF_HAND,
    WRISTS,
    BACK,
    NECK,
    LRING,
    RRING,
}

/**
 * PlayerInventory
 * Contains all top-level inventory slots for the player
 */
public class PlayerInventory
{
    Dictionary<PlayerInventorySlot, InventorySlot> m_inventory = new Dictionary<PlayerInventorySlot, InventorySlot>();

    public PlayerInventory(PlayerEntity owner) : base()
    {
        InitSlots(owner);
    }
    
    // Initialise each slot (TODO: Add slot restrictions)
    void InitSlots(PlayerEntity owner)
    {
        m_inventory[PlayerInventorySlot.HELD_ITEM] = new InventorySlot(owner);
        m_inventory[PlayerInventorySlot.OFF_HAND] = new InventorySlot(owner);
        m_inventory[PlayerInventorySlot.BAG_SLOT_1] = new InventorySlot(owner);
        m_inventory[PlayerInventorySlot.BAG_SLOT_2] = new InventorySlot(owner);
        m_inventory[PlayerInventorySlot.BAG_SLOT_3] = new InventorySlot(owner);
        m_inventory[PlayerInventorySlot.BAG_SLOT_4] = new InventorySlot(owner);
        m_inventory[PlayerInventorySlot.HEAD] = new InventorySlot(owner);
        m_inventory[PlayerInventorySlot.BODY] = new InventorySlot(owner);
        m_inventory[PlayerInventorySlot.LEGS] = new InventorySlot(owner);
        m_inventory[PlayerInventorySlot.FEET] = new InventorySlot(owner);
        m_inventory[PlayerInventorySlot.HANDS] = new InventorySlot(owner);
        m_inventory[PlayerInventorySlot.MAIN_HAND] = new InventorySlot(owner);
        m_inventory[PlayerInventorySlot.OFF_HAND] = new InventorySlot(owner);
        m_inventory[PlayerInventorySlot.WRISTS] = new InventorySlot(owner);
        m_inventory[PlayerInventorySlot.BACK] = new InventorySlot(owner);
        m_inventory[PlayerInventorySlot.NECK] = new InventorySlot(owner);
        m_inventory[PlayerInventorySlot.LRING] = new InventorySlot(owner);
        m_inventory[PlayerInventorySlot.RRING] = new InventorySlot(owner);
    }

    public InventorySlot GetSlot(PlayerInventorySlot slot)
    {
        return m_inventory[slot];
    }

    public InventorySlot this[PlayerInventorySlot slot]
    {
        get { return m_inventory[slot]; }
    }
}
