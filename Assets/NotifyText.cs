using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/** Text that appears at the bottom of the screen when examining something or when the player needs to be notified */
public class NotifyText : MonoBehaviour {

    [SerializeField]
    float m_textDuration = 4.0f;
    [SerializeField]
    float m_fadeOutTime = 1.0f;
    [SerializeField]
    float m_fadeInTime = 1.0f;

    [SerializeField]
    Color m_colour;

    Text m_text;

    enum TextTransitionState
    {
        HIDDEN,
        FADING_IN,
        OPAQUE,
        FADING_OUT
    }

    TextTransitionState m_state;
    // The colour of the text at the start of whatever transition it is currently in
    Color m_startColour;
    // The colour of the text at the end of whatever transition it is currently in
    Color m_endColour;
    // The current duration of the current transition
    float m_fadeCurrentDuration;
    // The final duration of the current transition
    float m_fadeEndDuration;
    
    /** Initilaise text to be blank and hidden */
	void Start () {
        m_text = GetComponent<Text>();

        if(m_text == null)
        {
            Debug.LogError("NotifyText gameobject requires a Text component.");
        }
        else
        {
            m_text.text = "";
            m_text.color = new Color(m_colour.r, m_colour.g, m_colour.b, 0.0f);
        }
	}
	
    /** Handle any transitions we want to do */
	void Update () {
        if (m_state != TextTransitionState.HIDDEN)
        {
            if (UpdateTransition())
            {
                m_fadeCurrentDuration = 0.0f;
                m_fadeEndDuration = 0.0f;
                // Transition finished - Move to the next state
                switch (m_state)
                {
                    case TextTransitionState.FADING_IN:
                        m_state = TextTransitionState.OPAQUE;
                        m_startColour = m_text.color;
                        m_endColour = m_text.color;
                        m_fadeEndDuration = m_textDuration;
                        break;
                    case TextTransitionState.OPAQUE:
                        m_state = TextTransitionState.FADING_OUT;
                        m_startColour = m_text.color;
                        m_endColour = new Color(m_startColour.r, m_startColour.g, m_startColour.b, 0.0f);
                        m_fadeEndDuration = m_fadeOutTime;
                        break;
                    case TextTransitionState.FADING_OUT:
                        m_state = TextTransitionState.HIDDEN;
                        break;
                }
            }
        }
	}

    /** Returns true if the transition is complete */
    bool UpdateTransition()
    {
        if (m_fadeCurrentDuration < m_fadeEndDuration)
        {
            m_fadeCurrentDuration += Time.deltaTime;
            float t = m_fadeCurrentDuration / m_fadeEndDuration;
            m_text.color = Color.Lerp(m_startColour, m_endColour, t);
        }

        return m_fadeCurrentDuration > m_fadeEndDuration;
    }

    public void SetText(string text)
    {
        m_text.text = text;
        
        m_state = TextTransitionState.FADING_IN;
        m_startColour = m_text.color;
        m_endColour = m_colour;

        m_fadeCurrentDuration = 0.0f;
        m_fadeEndDuration = m_fadeInTime;
    }
}
