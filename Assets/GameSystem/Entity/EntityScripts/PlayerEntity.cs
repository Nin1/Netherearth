using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Player Entity in the world.
 * Try not to have any direct input in this class
 */
public class PlayerEntity : WorldEntity {
    
    [Header("Interaction")]
    public float m_reach = 2.5f;
    public float m_dropDistance = 2.0f;
    public float m_dropForce = 100.0f;

    [Header("References")]
    public Transform m_eyePos; // This would be the camera object for the main player character
    CharacterController m_controller;

    // Runtime variables

    public PlayerInventory m_inventory;
    // The entity that the mouse was clicked-down on
    WorldEntity m_selectedEntity;
    // The world-space position of the mouse cursor
    Vector3 m_eyeDirection;

    public PlayerEntity() : base(BasicActionType.TALKTO, BasicActionType.EXAMINE, EntityType.PLAYER)
    {
        m_inventory = new PlayerInventory(this);
    }

    void Start()
    {
        m_eyePos = Camera.main.transform;
        m_controller = GetComponent<CharacterController>();
    }

    protected override bool PickUp(WorldEntity other)
    {
        // if (we are strong enough)
        //if (other.m_data.canPickUp)
        {
            SetHeldEntity(other);
        }
        return true;
    }

    /** Eye-based World Input */

    /** Return the player's eye position (camera position) */
    public Vector3 GetEyePos() { return m_eyePos.position; }

    /** Set the point in the world that the player eyes point towards */
    public void SetEyeDirection(Vector3 eyeDirection) { m_eyeDirection = eyeDirection; }
    
    /** Get the direction of the player's eyes (camera-to-mouse-cursor) */
    Vector3 GetEyeDirection() { return m_eyeDirection; }

    public void SetSelectedEntity(WorldEntity entity) { m_selectedEntity = entity; }
    public WorldEntity GetSelectedEntity() { return m_selectedEntity; }

    /** Returns the entity the player is looking at (camera-to-mouse-cursor) */
    public WorldEntity GetLookingAtEntity()
    {
        //int layerMask = LayerMask.GetMask("Player", "UI");
        RaycastHit hit;
        if (Physics.Raycast(m_eyePos.position, m_eyeDirection, out hit, m_reach))
        {
            return hit.transform.GetComponent<WorldEntity>();
        }
        return null;
    }

    /** Perform the default primary or secondary action on the entity under the mouse */
    public void InteractWithWorld(MouseInteractButton button)
    {
        WorldEntity subject = GetLookingAtEntity();
        if (subject)
        {
            if (button == MouseInteractButton.PRIMARY)
            {
                PerformPrimaryActionOn(subject);
            }
            else
            {
                PerformSecondaryActionOn(subject);
            }
        }
    }

    /** Try to pick up the given entity (e.g. from the inventory)
      * Returns true if the player successfully picks up the entity. */
    public bool TryPickUp(WorldEntity entity)
    {
        if(entity == null)
        {
            return false;
        }
        if (IsHoldingEntity())
        {
            return false;
        }
        return PerformBasicActionOn(BasicActionType.PICKUP, entity);
    }

    /** Try to pick up whatever the player is looking at. Returns false if they failed to pick up */
    public bool TryPickUp()
    {
        return TryPickUp(GetLookingAtEntity());
    }

    /** If the player is holding an item, try to drop it. Returns false if the drop failed. */
    public bool TryDropHeldEntity()
    {
        if (!IsHoldingEntity())
        {
            return false;
        }
        
        //int layerMask = LayerMask.GetMask("Player", "UI");
        RaycastHit hit;
        if (Physics.Raycast(m_eyePos.position, m_eyeDirection, out hit, m_dropDistance))
        {
            // An item can't be dropped here (e.g. wall is too close)
            return false;
        }
        else
        {
            // Drop the item in front of the player
            Vector3 dropPos = m_eyePos.transform.position + (m_eyeDirection * m_dropDistance);

            WorldEntity heldEntity = GetHeldEntity();
            heldEntity.gameObject.SetActive(true);
            Transform droppedEntity = heldEntity.transform;
            droppedEntity.parent = transform.parent;
            droppedEntity.position = dropPos;

            // @TODO: Hold-down interact and release for a more powerful throw
            Rigidbody rb = droppedEntity.GetComponent<Rigidbody>();
            if (rb)
            {
                rb.velocity = Vector3.zero;
                rb.AddForce(m_eyeDirection * m_dropForce);
            }


            // Not holding an item anymore!
            m_inventory[PlayerInventorySlot.HELD_ITEM].ClearSlot();
            return true;
        }
    }

    bool SetHeldEntity(WorldEntity entity)
    {
        // Can only set held item data if an item isn't currently held
        if (!IsHoldingEntity())
        {
            m_inventory[PlayerInventorySlot.HELD_ITEM].SetEntity(entity);
            // TODO: Replace SetActive with WorldEntity::HideFromWorld()
            entity.gameObject.SetActive(false);
            // Position entity to be physically "inside" the player - This could later be a hand position
            entity.transform.parent = this.transform;
            entity.transform.localPosition = new Vector3(0, 0, 0);
            SoftwareMouse.SetCursorImage(entity.m_data.sprite);
            return true;
        }
        return false;
    }
    
    public WorldEntity GetHeldEntity() { return m_inventory[PlayerInventorySlot.HELD_ITEM].GetEntity(); }
    public bool IsHoldingEntity() { return GetHeldEntity() != null; }

    // Dangerous!
    public void ClearHeldEntity()
    {
        m_inventory[PlayerInventorySlot.HELD_ITEM].ClearSlot();
        SoftwareMouse.SetCursorToDefault();
    }

    public InventorySlot GetInventorySlot(PlayerInventorySlot slot) { return m_inventory[slot]; }
}
