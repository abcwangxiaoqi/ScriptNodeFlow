using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class EditorStylesConfig : ScriptableObject 
{
    public List<GUIStyle> styles = new List<GUIStyle>();
}
