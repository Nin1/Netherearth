using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventorySlotInput : MonoBehaviour, IPointerDownHandler, IDragHandler, IDropHandler, IPointerUpHandler
{

    Image m_image;
    InventorySlot m_slot;
    PlayerEntity m_player;

    public void Init(PlayerEntity player, InventorySlot slot)
    {
        m_image = GetComponent<Image>();
        m_player = player;
        m_slot = slot;
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("Mouse drag on inventory slot!");
        if (m_player.TryPickUp(m_slot.GetEntity()))
        {
            // The player has picked up our item
            m_slot.ClearSlot();
            m_image.color = Color.clear;
            eventData.pointerDrag = null;
            eventData.dragging = false;
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        // If the player is holding an item, try putting it in the slot.
        if (m_slot.IsEmpty())
        {
            Debug.Log("Trying to put item in inventory slot");
            if (m_slot.SetEntity(m_player.GetHeldEntity()))
            {
                Debug.Log("Put item in inventory slot");
                m_image.sprite = m_slot.GetEntity().m_data.sprite;
                m_image.color = Color.white;
                m_player.ClearHeldEntity();
                return;
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.pointerEnter == this.gameObject)
        {
            // If the player is holding an item, try putting it in the slot.
            if (m_slot.IsEmpty())
            {
                Debug.Log("Trying to put item in inventory slot");
                if (m_slot.SetEntity(m_player.GetHeldEntity()))
                {
                    Debug.Log("Put item in inventory slot");
                    m_image.sprite = m_slot.GetEntity().m_data.sprite;
                    m_image.color = Color.white;
                    m_player.ClearHeldEntity();
                    return;
                }
            }
            // Try to "use" whatever is in the slot
            WorldEntity entity = m_slot.GetEntity();
            if (entity && m_slot.m_owner == m_player)
            {
                m_player.PerformPrimaryActionOn(entity);
                return;
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {

    }
}
