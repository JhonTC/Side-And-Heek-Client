using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PaintManager))]
public class PaintManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        PaintManager paintManager = (PaintManager)target;

        if (GUILayout.Button("Register New Paint"))
        {
            //paintManager.RegisterPosition((ushort)(PaintManager.paintLocations.Count + 1), new Vector3(0, 0, 0));
        }

        if (GUILayout.Button("Update Paint"))
        {
            paintManager.UpdatePaint();
        }
    }
}
