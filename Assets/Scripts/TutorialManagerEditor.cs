using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TutorialManager))]
public class TutorialManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        TutorialManager tutorial = (TutorialManager)target;

        if (GUILayout.Button("Play Next Tutorial Message"))
        {
            tutorial.PlayNextMessage(true);
        }
        if (GUILayout.Button("Click Trigger"))
        {
            tutorial.onClickTrigger();
        }
        if (GUILayout.Button("Play Next Mid Message"))
        {
            tutorial.PlayNextMessage(false);
        }
    }

}
