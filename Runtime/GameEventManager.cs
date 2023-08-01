// Copyright (C) 2022 Alejandro Güereca

using UnityEngine;
using System;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

namespace Vocario.EventBasedArchitecture
{
    [CreateAssetMenu(fileName = "GameEventManager", menuName = "Vocario/GameEventManager", order = 0)]
    public class GameEventManager : ScriptableObject
    {
        protected static GameEventManager _instance;
        public static GameEventManager Instance
        {
            get
            {
                TrySetInstance();

                return _instance ?? throw new Exception("Create a game event manager before referencing it in runtime");
            }
        }

        protected static void TrySetInstance()
        {
            if (_instance == null)
            {
                GameEventManager[] instances = Resources.FindObjectsOfTypeAll<GameEventManager>();
                _instance = (instances.Length > 0) ? instances[ 0 ] : null;
            }
        }

#if UNITY_EDITOR

        [UnityEditor.Callbacks.DidReloadScripts]
        private static void RefreshEventsOnScriptsReload()
        {
            TrySetInstance();
            if (_instance == null)
            {
                return;
            }
            if (EditorApplication.isCompiling || EditorApplication.isUpdating)
            {
                EditorApplication.delayCall += RefreshEventsOnScriptsReload;
                return;
            }

            EditorApplication.delayCall += RefreshEventsCall;
        }

        public static void RefreshEventsCall() => RefreshEvents();


        public static bool RefreshEvents()
        {
            var eventInstances = AppDomain.CurrentDomain.GetAssemblies()
                .Aggregate(new List<Type>(), (accumulator, currentAssembly) =>
                {
                    foreach (Type type in currentAssembly.GetTypes())
                    {
                        accumulator.Add(type);
                    }
                    return accumulator;
                })
                .Where(type => type.IsClass && type.IsPublic && !type.IsAbstract && type.IsSubclassOf(typeof(AGameEvent)))
                .Select(type =>
                {
                    var ev = (AGameEvent) Activator.CreateInstance(type);
                    return ev;
                })
                .ToList();


            var newMap = new EventsMap();
            foreach (AGameEvent gameEvent in eventInstances)
            {
                newMap.Add(gameEvent.GetType().ToString(), gameEvent);
            }

            if (newMap == Instance._events)
            {
                return false;
            }

            Instance._events = newMap;
            EditorUtility.SetDirty(Instance);
            AssetDatabase.SaveAssets();
            return true;
        }

#endif

        [SerializeField]
        protected EventsMap _events;
        public EventsMap Events => _events;

        // TODO Create event not found exception
        protected AGameEvent GetGameEvent<TEvent>() where TEvent : AGameEvent
        {
            string eventName = typeof(TEvent).ToString();
            if (!_events.ContainsKey(eventName))
            {
                Debug.LogError($"Get Event Failed: could not find event with ID {eventName}.");
                return null;
            }

            return _events[ eventName ];
        }

        public static bool RaiseEvent<TEvent, TParams>(TParams callParams)
            where TEvent : AGameEvent<TParams>
            where TParams : struct
        {
            var gameEvent = (AGameEvent<TParams>) Instance.GetGameEvent<TEvent>();
            if (gameEvent == null)
            {
                return false;
            }

            ((AGameEvent<TParams>) Instance._events[ gameEvent.Name ]).Invoke(callParams);
            return true;
        }

        public static bool AddListener<TEvent, TParams>(object parent, Action<TParams> handle)
            where TEvent : AGameEvent<TParams>
            where TParams : struct
        {
            var gameEvent = (AGameEvent<TParams>) Instance.GetGameEvent<TEvent>();
            if (gameEvent == null)
            {
                return false;
            }
            var listener = new GameEventListener<TParams>(gameEvent, parent, handle);

            return ((AGameEvent<TParams>) Instance._events[ gameEvent.Name ]).Register(listener);
        }

        public static bool RemoveListener<TEvent, TParams>(object parent, Action<TParams> handle)
            where TEvent : AGameEvent<TParams>
            where TParams : struct
        {
            var gameEvent = (AGameEvent<TParams>) Instance.GetGameEvent<TEvent>();
            if (gameEvent == null)
            {
                return false;
            }
            var listener = new GameEventListener<TParams>(gameEvent, parent, handle);

            return ((AGameEvent<TParams>) Instance._events[ gameEvent.Name ]).Deregister(listener);
        }

        public static bool RaiseEvent<TEvent>()
            where TEvent : AGameEvent
        {
            AGameEvent gameEvent = Instance.GetGameEvent<TEvent>();
            if (gameEvent == null)
            {
                return false;
            }

            Instance._events[ gameEvent.Name ].Invoke();
            return true;
        }

        public static bool AddListener<TEvent>(object parent, Action handle)
            where TEvent : AGameEvent
        {
            AGameEvent gameEvent = Instance.GetGameEvent<TEvent>();
            if (gameEvent == null)
            {
                return false;
            }
            var listener = new GameEventListener(gameEvent, parent, handle);

            return Instance._events[ gameEvent.Name ].Register(listener);
        }

        public static bool RemoveListener<TEvent>(object parent, Action handle)
            where TEvent : AGameEvent
        {
            AGameEvent gameEvent = Instance.GetGameEvent<TEvent>();
            if (gameEvent == null)
            {
                return false;
            }
            var listener = new GameEventListener(gameEvent, parent, handle);

            return Instance._events[ gameEvent.Name ].Deregister(listener);
        }

        public static bool RemoveAllListeners<TEvent>()
            where TEvent : AGameEvent
        {
            AGameEvent gameEvent = Instance.GetGameEvent<TEvent>();
            if (gameEvent == null)
            {
                return false;
            }
            gameEvent.DeregisterAll();
            return true;
        }
    }

    [Serializable]
    public class EventsMap : SerializableDictionary<string, AGameEvent> { }
}


