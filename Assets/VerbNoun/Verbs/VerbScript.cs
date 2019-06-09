using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

// Maybe this can be done the other way around? i.e. BaseVerb, where it contains a function for each noun instead and has its own default?
public abstract class VerbScript
{
    // Returns the interaction implemented for the given noun
    public Interaction GetInteractionForNoun(Noun n)
    {
        // Todo: maybe get function by name (From noun name) using some weird c# stuff
        switch (n.m_name)
        {
            case "door":
                return GetInteraction(Door);
            case "rock":
                return GetInteraction(Rock);
            case "puddle":
                return GetInteraction(Puddle);
            default:
                return DefaultInteraction;
        }
    }

    // Return either the noun's interaction (if it has been overridden), or the verb's default interaction
    Interaction GetInteraction(Interaction e)
    {
        // Spooky reflection stuff to check if the method has been overridden
        if (e.Method.GetBaseDefinition().DeclaringType != e.Method.DeclaringType)
        {
            // This method has been overridden by the child class, indicating that it has a unique implementation
            return e;
        }
        else
        {
            return DefaultInteraction;
        }
    }

    /** Default action that happens on any object where an interaction is not defined */

    protected abstract void DefaultInteraction(InteractibleObject obj);

    /** Functions for this verb's action on each noun */

    void Door(InteractibleObject obj) { }
    void Rock(InteractibleObject obj) { }
    void Puddle(InteractibleObject obj) { }

}
