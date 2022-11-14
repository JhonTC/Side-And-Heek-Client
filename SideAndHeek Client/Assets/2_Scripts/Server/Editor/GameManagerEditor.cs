using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Server
{
    [CustomEditor(typeof(LocalGameManager))]
    public class GameManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            LocalGameManager gameManager = (LocalGameManager)target;

            base.OnInspectorGUI();

            if (GUILayout.Button("END GAME"))
            {
                //gameManager.GameOver(true);
            }
        }
    }
}
