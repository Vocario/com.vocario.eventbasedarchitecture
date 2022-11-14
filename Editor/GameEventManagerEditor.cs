using UnityEngine;
using UnityEditor;

namespace Vocario.EventBasedArchitecture
{
    [CustomEditor(typeof(GameEventManager))]
    public class GameEventManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            // TODO Add enum creation and generate C# code options
            base.OnInspectorGUI();
            // DisplayEvents();
            var manager = target as GameEventManager;

            if (GUILayout.Button("Refresh events"))
            {
                manager.ReloadEvents();
            }

            if (GUILayout.Button("Force save"))
            {
                EditorUtility.SetDirty(manager);
                AssetDatabase.SaveAssets();
            }
        }

        private void DisplayEvents()
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
