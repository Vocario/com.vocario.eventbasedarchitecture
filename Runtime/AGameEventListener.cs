// Copyright (C) 2022 Alejandro Güereca

using UnityEngine;
using System;

namespace Vocario.EventBasedArchitecture
{
    [Serializable]
    public abstract class AGameEventListener
    {
        [SerializeReference, HideInInspector]
        protected GameEvent _gameEvent;
        protected AGameEventListener(GameEvent gameEvent) => _gameEvent = gameEvent;

        public abstract void RaiseEvent();
        public void Register() => _gameEvent?.Register(this);
        public void Deregister() => _gameEvent?.Deregister(this);
    }
}
