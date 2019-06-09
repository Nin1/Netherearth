using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateNormal : PlayerState {

    public override PlayerStateName state { get { return PlayerStateName.NORMAL; } }

    // Called immediately when changing to this state
    public override void OnChangeTo(PlayerController pc)
    {

    }

    // Called immediately when changing from this state
    public override void OnChangeFrom(PlayerController pc)
    {

    }
}
