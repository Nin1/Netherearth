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
        if (m_player.TryPickUp(m_slot.GetEntity()))
        {
            // The player has picked up our item
            RemoveItemFromSlot();
            eventData.pointerDrag = null;
            eventData.dragging = false;
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        TryPutItemInSlot();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.pointerEnter == this.gameObject)
        {
            // If the player is holding an item, try putting it in the slot.
            if (TryPutItemInSlot())
            {
                return;
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

    /** Try to put the player's held object in this slot. Return true on success. */
    bool TryPutItemInSlot()
    {
        if (m_slot.IsEmpty())
        {
            if (m_slot.SetEntity(m_player.GetHeldEntity()))
            {
                m_player.ClearHeldEntity();
                m_image.color = Color.white;
                m_image.sprite = m_slot.GetEntity().m_data.sprite;
                m_slot.GetEntity().AddOnDestructListener(OnItemDestroyed);

                return true;
            }
        }
        return false;
    }

    void RemoveItemFromSlot()
    {
        m_slot.GetEntity().RemoveOnDestructListener(OnItemDestroyed);
        m_slot.ClearSlot();
        m_image.color = Color.clear;
    }

    /** Delegate to listen to when the slot's item is destroyed so we can clear the sprite */
    void OnItemDestroyed()
    {
        m_image.color = Color.clear;
    }

    void OnDestroy()
    {
        // Stop listening to item before we are destroyed
        if(m_slot != null && m_slot.GetEntity())
        {
            m_slot.GetEntity().RemoveOnDestructListener(OnItemDestroyed);
        }
    }
}
