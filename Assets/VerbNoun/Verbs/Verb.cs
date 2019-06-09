using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Verb", menuName = "Verb", order = 1)]
[System.Serializable]
public class Verb : ScriptableObject {
    public string m_name;
    public List<string> m_synonyms = new List<string>();
    public string m_scriptName;
}
