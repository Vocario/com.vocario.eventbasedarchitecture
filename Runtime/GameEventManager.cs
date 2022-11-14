// Copyright (C) 2022 Alejandro Güereca

using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

// TODO Make scriptable object dependant of specified enum through editor, maybe even create a enum based static getter?
// TODO Reload events on script compilations
namespace Vocario.EventBasedArchitecture
{
    [CreateAssetMenu(fileName = "GameEventManager", menuName = "Vocario/GameEventManager", order = 0)]
    public class GameEventManager : ScriptableObject
    {
        [SerializeField]
        private EventsMap _events;

        public void ReloadEvents()
        {
            _events = new EventsMap();
            IEnumerable<Type> enumTypes = AppDomain.CurrentDomain
                .GetAssemblies()
                .Select(assembly => assembly.GetTypes())
                .SelectMany(x => x)
                .Where(type => type.IsDefined(typeof(GameEventsAttribute), false));

            foreach (Type enumType in enumTypes)
            {
                string[] eventIds = Enum.GetNames(enumType);

                foreach (string eventId in eventIds)
                {
                    _events.Add((Enum) Enum.Parse(enumType, eventId), new GameEvent(eventId));
                }
            }
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }

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
