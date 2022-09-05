// Copyright (C) 2022 Alejandro Güereca

using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Vocario.EventBasedArchitecture
{
    [CreateAssetMenu(fileName = "GameEventManager", menuName = "Vocario/GameEventManager", order = 0)]
    public class GameEventManager : ScriptableObject
    {
        // TODO Add serializable dictionary
        private Dictionary<Enum, GameEvent> _events = new Dictionary<Enum, GameEvent>();

        private void OnEnable()
        {
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
                    _events.Add((Enum) Enum.Parse(enumType, eventId), new GameEvent());
                }

            }
        }

        public void RaiseEvent(Enum eventId)
        {
            if (!_events.ContainsKey(eventId))
            {
                Debug.LogError($"Event Raised Failed: could not find event with ID {eventId}.");
                return;
            }
            _events[ eventId ].Invoke();
            Debug.Log($"{eventId} - Event Raised");
        }

        public AGameEventListener CreateListener(Enum eventId, Action onEventRaised)
        {
            if (!_events.ContainsKey(eventId))
            {
                Debug.LogError($"Event Register Failed: could not find event with ID {eventId}.");
                return null;
            }
            GameEvent gameEvent = _events[ eventId ];
            // TODO: Add some object pooling for the listener maybe
            var listener = new GameEventListener(gameEvent, onEventRaised);

            _ = _events[ eventId ].Register(listener);
            return listener;
        }
    }
}
