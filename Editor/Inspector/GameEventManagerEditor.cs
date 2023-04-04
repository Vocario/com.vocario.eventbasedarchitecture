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
            _manager = target as GameEventManager;
            _options = GetAvailableEnums();
            _manager.Load(_options[ _indexTypeSelected ]);
        }

        public override void OnInspectorGUI()
        {
            // TODO Add enum creation and generate C# code options
            base.OnInspectorGUI();
            // DisplayEvents();
            DisplayEventEnumSelection();

            if (GUILayout.Button("Refresh events"))
            {
                _manager.RefreshEvents();
            }

            if (GUILayout.Button("Force save"))
            {
                EditorUtility.SetDirty(_manager);
                AssetDatabase.SaveAssets();
            }
        }

        protected void DisplayEventEnumSelection()
        {
            EditorGUI.BeginChangeCheck();
            string[] dropdownOptions = _options.Select(x => x.ToString()).ToArray();
            _indexTypeSelected = EditorGUILayout.Popup("Available event groups", _indexTypeSelected, dropdownOptions);

            if (EditorGUI.EndChangeCheck())
            {
                _manager.Load(_options[ _indexTypeSelected ]);
            }
        }

        protected List<Type> GetAvailableEnums() => AppDomain.CurrentDomain
                .GetAssemblies()
                .Select(assembly => assembly.GetTypes())
                .SelectMany(x => x)
                .Where(type => type.GetCustomAttributes(typeof(GameEventsAttribute), true).Length > 0)
                .ToList();

        protected void DisplayEvents()
        {
            SerializedProperty events = serializedObject.FindProperty("_events").Copy();
            SerializedProperty values = events.FindPropertyRelative("m_values");

            GUILayout.BeginVertical();
            while (values.Next(true))
            {
                if (values.name == "_name")
                {
                    EditorGUILayout.LabelField(values.stringValue);
                }
            }
            GUILayout.EndVertical();
        }
    }
}
