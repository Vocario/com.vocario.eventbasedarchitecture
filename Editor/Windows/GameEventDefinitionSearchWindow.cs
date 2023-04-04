using UnityEngine;
using UnityEditor.Experimental.GraphView;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEditor;
using Vocario.EventBasedArchitecture;

public class GameEventDefinitionSearchWindow : ScriptableObject, ISearchWindowProvider
{
    private GameEventManager _eventManager;

    internal void Init(GameEventManager eventManager) => _eventManager = eventManager;

    // TODO Cache and refetch on change
    public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
    {
        Texture icon = EditorGUIUtility.FindTexture("d_cs Script Icon");
        var header = new List<SearchTreeEntry>() { new SearchTreeGroupEntry(new GUIContent("State Behaviours")) };
        IEnumerable<SearchTreeEntry> searchTreeEntries = AppDomain.CurrentDomain
            .GetAssemblies()
            .Select(assembly => assembly.GetTypes())
            .SelectMany(x => x)
            .Where(type => type.GetCustomAttributes(typeof(GameEventsAttribute), true).Length > 0)
            .Select(type => new SearchTreeEntry(new GUIContent(type.ToString(), icon))
            {
                userData = type,
                level = 1
            });

        return header.Concat(searchTreeEntries).ToList();
    }

    public bool OnSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context)
    {
        return true;
    }
}
