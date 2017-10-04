using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AFArcade {

public static class EditorGUITools
{
    private static readonly Texture2D backgroundTexture = Texture2D.whiteTexture;
    private static readonly GUIStyle textureStyle = new GUIStyle {normal = new GUIStyleState { background = backgroundTexture } };
 
    public static void DrawRect(Rect position, Color color, GUIContent content = null)
    {
        var backgroundColor = GUI.backgroundColor;
        GUI.backgroundColor = color;
        textureStyle.alignment = TextAnchor.MiddleCenter;
        textureStyle.fontStyle = FontStyle.Bold;
        GUI.Box(position, content ?? GUIContent.none, textureStyle);
        GUI.backgroundColor = backgroundColor;
    }
 
    public static void LayoutBox(Color color, GUIContent content = null)
    {
        var backgroundColor = GUI.backgroundColor;
        GUI.backgroundColor = color;
        GUILayout.Box(content ?? GUIContent.none, textureStyle);
        GUI.backgroundColor = backgroundColor;
    }
}

}