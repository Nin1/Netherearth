using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HolyCubeEntity : WorldEntity
{
    public HolyCubeEntity() : base(BasicActionType.USE, BasicActionType.EXAMINE, EntityType.ITEM)
    {
    }

    protected override void OnPickUp(WorldEntity other)
    {
        EntityStat otherHealth = other.GetStat(EntityStatType.HEALTH);
        if (otherHealth != null)
        {
            otherHealth.Decrease(1);
            TopLevelUI.notifyText.SetText("You feel a jolt of pain flow through your body as you lift the cube.");
        }
    }
}
