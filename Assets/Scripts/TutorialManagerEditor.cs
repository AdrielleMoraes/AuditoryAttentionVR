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

        if (GUILayout.Button("Play Next Message"))
        {
            tutorial.Start_Coroutine();
        }
    }

}
