using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Vocario.EventBasedArchitecture
{
    [CustomEditor(typeof(GameEventManager), editorForChildClasses: true)]
    public class GameEventManagerEditor : Editor
    {
        private int _indexTypeSelected = 0;
        private GameEventManager _manager = null;
        private List<Type> _options = null;


        private void OnEnable()
        {
        }

        public override void OnInspectorGUI()
        {
            // TODO Add enum creation and generate C# code options
            base.OnInspectorGUI();

            if (GUILayout.Button("Force Save"))
            {
                EditorUtility.SetDirty(target);
                AssetDatabase.SaveAssets();
            }
        }
    }
}
