// Copyright (C) 2022 Alejandro Güereca

using UnityEngine;
using System;

namespace Vocario.EventBasedArchitecture
{
    [Serializable]
    public abstract class AGameEventListener
    {
        [SerializeField]
        protected string _eventTypeName;

        protected AGameEventListener(AGameEvent gameEvent) => _eventTypeName = gameEvent.Name;
        public abstract void RaiseEvent();
    }

    [Serializable]
    public abstract class AGameEventListener<T> : AGameEventListener where T : struct
    {
        protected AGameEventListener(AGameEvent gameEvent) : base(gameEvent) { }

        public abstract void RaiseEvent(T param);
    }
}
