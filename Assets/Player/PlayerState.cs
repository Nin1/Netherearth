using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerStateName
{
    NORMAL,
    HOLDING_ITEM
}

public abstract class PlayerState {

    public abstract PlayerStateName state { get; } 

    // Called immediately when changing to this state
    public abstract void OnChangeTo(PlayerController pc);



    // Called immediately when changing from this state
    public abstract void OnChangeFrom(PlayerController pc);
}
