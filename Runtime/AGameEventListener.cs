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
        protected AGameEventListener(AGameEvent gameEvent) => _gameEvent = gameEvent;
        public void Register() => _gameEvent?.Register(this);
        public void Deregister() => _gameEvent?.Deregister(this);
        public abstract void RaiseEvent();
    }

    [Serializable]
    public abstract class AGameEventListener<T> : AGameEventListener where T : struct
    {
        protected AGameEventListener(AGameEvent gameEvent) : base(gameEvent) { }

        public abstract void RaiseEvent(T param);
    }
}
