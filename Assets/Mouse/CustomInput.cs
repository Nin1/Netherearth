using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CustomInput : BaseInput {

    public override Vector2 mousePosition
    {
        get
        {
            return SoftwareMouse.GetMousePos();
        }
    }
}
