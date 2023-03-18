// Copyright (C) 2022 Alejandro Güereca

using UnityEngine;
using System;
using UnityEditor;

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
            _events = new EventsMap();
            string[] eventIds = Enum.GetNames(_enumType);

            foreach (string eventId in eventIds)
            {
                _events.Add((Enum) Enum.Parse(_enumType, eventId), new GameEvent(eventId));
            }
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }
#endif

        // TODO Remove code repetition
        // TODO Create event not found exception
        public void RaiseEvent(Enum eventId)
        {
            int id = Convert.ToInt32(eventId);
            if (!_events.ContainsKey(id))
            {
                Debug.LogError($"Event Raised Failed: could not find event with ID {eventId}.");
                return;
            }
            _events[ id ].Invoke();
            Debug.Log($"{eventId} - Event Raised");
        }

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
