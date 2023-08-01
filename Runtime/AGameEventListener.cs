// Copyright (C) 2022 Alejandro Güereca

using UnityEngine;
using System;

namespace Vocario.EventBasedArchitecture
{
    [Serializable]
    public abstract class AGameEventListener
    {
        [SerializeReference, HideInInspector]
        protected AGameEvent _gameEvent;

        public string GameEventName => _gameEvent.Name;
    }
    [Serializable]
    public abstract class AGameEventListener<T> : AGameEventListener where T : struct
    {
        protected AGameEventListener(AGameEvent<T> gameEvent) => _gameEvent = gameEvent;

        public abstract void RaiseEvent(T param);
        public void Register() => (_gameEvent as AGameEvent<T>)?.Register(this);
        public void Deregister() => (_gameEvent as AGameEvent<T>)?.Deregister(this);
    }
}
