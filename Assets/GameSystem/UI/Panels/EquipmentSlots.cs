using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentSlots : MonoBehaviour
{
    [SerializeField]
    InventorySlotInput m_headSlot;
    [SerializeField]
    InventorySlotInput m_bodySlot;
    [SerializeField]
    InventorySlotInput m_legsSlot;
    [SerializeField]
    InventorySlotInput m_feetSlot;
    [SerializeField]
    InventorySlotInput m_handsSlot;
    [SerializeField]
    InventorySlotInput m_mainHandSlot;
    [SerializeField]
    InventorySlotInput m_offHandSlot;
    [SerializeField]
    InventorySlotInput m_wristsSlot;
    [SerializeField]
    InventorySlotInput m_neckSlot;
    [SerializeField]
    InventorySlotInput m_backSlot;
    [SerializeField]
    InventorySlotInput m_lRingSlot;
    [SerializeField]
    InventorySlotInput m_rRingSlot;
    [SerializeField]
    PlayerEntity m_player;


    // Use this for initialization
    void Awake()
    {
        if (!m_player)
        {
            m_player = FindObjectOfType<PlayerEntity>();
        }
        m_headSlot.Init(m_player, m_player.m_inventory[PlayerInventorySlot.HEAD]);
        m_bodySlot.Init(m_player, m_player.m_inventory[PlayerInventorySlot.BODY]);
        m_legsSlot.Init(m_player, m_player.m_inventory[PlayerInventorySlot.LEGS]);
        m_feetSlot.Init(m_player, m_player.m_inventory[PlayerInventorySlot.FEET]);
        m_handsSlot.Init(m_player, m_player.m_inventory[PlayerInventorySlot.HANDS]);
        m_mainHandSlot.Init(m_player, m_player.m_inventory[PlayerInventorySlot.MAIN_HAND]);
        m_offHandSlot.Init(m_player, m_player.m_inventory[PlayerInventorySlot.OFF_HAND]);
        m_wristsSlot.Init(m_player, m_player.m_inventory[PlayerInventorySlot.WRISTS]);
        m_neckSlot.Init(m_player, m_player.m_inventory[PlayerInventorySlot.NECK]);
        m_backSlot.Init(m_player, m_player.m_inventory[PlayerInventorySlot.BACK]);
        m_lRingSlot.Init(m_player, m_player.m_inventory[PlayerInventorySlot.LRING]);
        m_rRingSlot.Init(m_player, m_player.m_inventory[PlayerInventorySlot.RRING]);
    }
}