// Copyright (C) 2022 Alejandro Güereca

using UnityEngine;
using System;
using UnityEngine.Events;

namespace Vocario.EventBasedArchitecture
{
    [Serializable]
    public abstract class AGameEventListener
    {
        [SerializeField]
        protected int _parentHash;
        [SerializeField]
        protected string _eventTypeName;
        [SerializeField]
        protected UnityEventBase _onEventRaised;

        protected AGameEventListener(AGameEvent gameEvent, object parent)
        {
            _eventTypeName = gameEvent.Name;
            _parentHash = parent.GetHashCode();
        }

        public override int GetHashCode() => HashCode.Combine(_parentHash, _eventTypeName);
    }

    [Serializable]
    public abstract class AGameEventListener<T> : AGameEventListener where T : struct

    {
        protected AGameEventListener(AGameEvent gameEvent, object parent) : base(gameEvent, parent) { }

        public abstract void RaiseEvent(T param = default);
    }
}
