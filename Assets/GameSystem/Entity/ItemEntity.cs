using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemEntity : WorldEntity {
    
    public SpriteRenderer m_sprite;
    //Rigidbody m_rb;

    public ItemEntity() : base(ActionType.NONE, ActionType.EXAMINE)
    {
        m_entityType = EntityType.ITEM;
    }

    protected override void OnPickUp(WorldEntity other)
    {
        /*
        if (other.m_entityType == EntityType.PLAYER)
        {
            // Give player item
            PlayerEntity player = other as PlayerEntity;
            if (player.SetHeldEntity(this))
            {
            }
        }
        */
    }

    public void SetData(EntityData data)
    {
        m_data = data;
        m_sprite.sprite = m_data.sprite;
        // Set rigidbody mass, etc.
    }

}
