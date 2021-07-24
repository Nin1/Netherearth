using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodEntity : WorldEntity
{
    public SpriteRenderer m_sprite;
    // FoodData m_data override EntityData???

    public FoodEntity() : base(BasicActionType.ACTIVATE, BasicActionType.EXAMINE, EntityType.ITEM)
    {
    }

    private void Start()
    {
        // TODO: Make this automagical when setting EntityData
        m_sprite.sprite = m_data.sprite;
    }

    protected override void OnActivated(WorldEntity other)
    {
        // Get how much health this food will heal
        EntityStat healingAmount = this.GetStat(EntityStatType.HEALTH);
        EntityStat otherHealth = other.GetStat(EntityStatType.HEALTH);
        if (otherHealth != null && healingAmount != null)
        {
            otherHealth.Increase(healingAmount.m_currentValue);
        }

        // Get how much hunger this food satiates
        EntityStat hungerAmount = this.GetStat(EntityStatType.HUNGER);
        EntityStat otherHunger = other.GetStat(EntityStatType.HUNGER);
        if (otherHunger != null && hungerAmount != null)
        {
            otherHunger.Increase(otherHunger.m_currentValue);
        }
        
        TopLevelUI.notifyText.SetText("The banana gently energises you.");

        Destroy(gameObject);
    }

    protected override void OnEquipped(WorldEntity other, InventorySlot slot)
    {
        if ((slot.m_equipMask & EquipMask.HEAD) > 0)
        {
            // Make the other entity eat this food if equipped to the head slot
            other.PerformBasicActionOn(BasicActionType.ACTIVATE, this);
        }
    }
}
