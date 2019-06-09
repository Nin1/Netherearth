using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractibleObject : MonoBehaviour {

    public Noun m_noun;

    public void PerformAction(VerbScript verb)
    {
        // Get the function corresponding to this particular verb/noun pair
        Interaction action = verb.GetInteractionForNoun(m_noun);
        // Perform the function
        action(this);
    }
}
