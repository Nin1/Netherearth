using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalkTo : VerbScript {

    /** Default action that happens on any object where an interaction is not defined */

    protected override void DefaultInteraction(InteractibleObject obj)
    {
        Debug.Log("The " + obj.m_noun.m_name + " does not have anything to say to you.");
    }

    /** Functions for this verb's action on each noun */
    
    void Rock(InteractibleObject obj)
    {
        Debug.Log("The rock says it feels like it just gets stepped on.");
    }
}
