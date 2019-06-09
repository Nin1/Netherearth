using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BagSlots : MonoBehaviour {

    [SerializeField]
    InventorySlotInput m_bagSlot1;
    [SerializeField]
    InventorySlotInput m_bagSlot2;
    [SerializeField]
    InventorySlotInput m_bagSlot3;
    [SerializeField]
    InventorySlotInput m_bagSlot4;
    [SerializeField]
    PlayerEntity m_player;


    // Use this for initialization
    void Awake()
    {
        if (!m_player)
        {
            m_player = FindObjectOfType<PlayerEntity>();
        }
        m_bagSlot1.Init(m_player, m_player.m_inventory[PlayerInventorySlot.BAG_SLOT_1]);
        m_bagSlot2.Init(m_player, m_player.m_inventory[PlayerInventorySlot.BAG_SLOT_2]);
        m_bagSlot3.Init(m_player, m_player.m_inventory[PlayerInventorySlot.BAG_SLOT_3]);
        m_bagSlot4.Init(m_player, m_player.m_inventory[PlayerInventorySlot.BAG_SLOT_4]);
    }
}
