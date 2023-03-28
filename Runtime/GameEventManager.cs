// Copyright (C) 2022 Alejandro Güereca

using UnityEngine;
using System;
using UnityEditor;
using System.Collections.Generic;

// TODO Make scriptable object dependant of specified enum through editor, maybe even create a enum based static getter?
// TODO Reload events on script compilations
namespace Vocario.EventBasedArchitecture
{
    [CreateAssetMenu(fileName = "GameEventManager", menuName = "Vocario/GameEventManager", order = 0)]
    public class GameEventManager : ScriptableObject
    {
        [SerializeField]
        protected EventsMap _events;
        protected Type _enumType = null;
        public Type EnumType => _enumType;
        public EventsMap Events => _events;

        public void Load(Type enumType)
        {
            _enumType = enumType;
            RefreshEvents();
        }

#if UNITY_EDITOR
        // [UnityEditor.Callbacks.DidReloadScripts]
        // private void RefreshEventsOnScriptsReload()
        // {
        //     if (EditorApplication.isCompiling || EditorApplication.isUpdating)
        //     {
        //         EditorApplication.delayCall += RefreshEventsOnScriptsReload;
        //         return;
        //     }

        //     EditorApplication.delayCall += RefreshEvents;
        // }


        public void RefreshEvents()
        {
            var newMap = new EventsMap();
            string[] eventIds = Enum.GetNames(_enumType);

            foreach (string eventId in eventIds)
            {
                int eventIndex = (int) Enum.Parse(_enumType, eventId);
                if (_events != null && _events.ContainsKey(eventIndex))
                {
                    newMap.Add(eventIndex, _events[ eventIndex ]);
                    continue;
                }

                newMap.Add(eventIndex, new GameEvent(eventId));
            }
            _events = newMap;
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }
#endif

        public static void RaiseEvent(Enum eventId)
        {
            GameEventManager eventManager = GetEventManager(eventId.GetType());
            GameEvent gameEvent = eventManager.GetGameEvent(eventId);
            gameEvent.Invoke();
            Debug.Log($"{eventId} - Event Raised");
        }

        protected static Dictionary<Type, GameEventManager> _cachedEventManagers = new Dictionary<Type, GameEventManager>();

        protected static GameEventManager GetEventManager(Type enumType)
        {
            if (_cachedEventManagers.ContainsKey(enumType))
            {
                return _cachedEventManagers[ enumType ];
            }

            string[] guids = AssetDatabase.FindAssets($"t:{typeof(GameEventManager).Name}");  //FindAssets uses tags check documentation for more info

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                GameEventManager eventManager = AssetDatabase.LoadAssetAtPath<GameEventManager>(path);

                if (eventManager.EnumType == enumType)
                {
                    _cachedEventManagers[ enumType ] = eventManager;
                    return eventManager;
                }
            }

            // TODO Throw event not found exception
            return null;
        }

        // TODO Create event not found exception
        public GameEvent GetGameEvent(Enum eventId)
        {
            int id = Convert.ToInt32(eventId);
            if (!_events.ContainsKey(id))
            {
                Debug.LogError($"Get Event Failed: could not find event with ID {eventId}.");
                return null;
            }

            return _events[ id ];
        }

        public AGameEventListener CreateListener(Enum eventId, Action onEventRaised)
        {
            int id = Convert.ToInt32(eventId);
            if (!_events.ContainsKey(id))
            {
                Debug.LogError($"Event Register Failed: could not find event with ID {eventId}.");
                return null;
            }
            GameEvent gameEvent = _events[ id ];
            // TODO: Add some object pooling for the listener maybe
            var listener = new GameEventListener(gameEvent, onEventRaised);

            _ = _events[ id ].Register(listener);
            return listener;
        }
    }

    [Serializable]
    public class EventsMap : SerializableDictionary<int, GameEvent> { }
}
