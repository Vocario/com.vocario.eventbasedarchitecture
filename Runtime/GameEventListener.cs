// Copyright (C) 2022 Alejandro Güereca

using System;

using UnityEditor.Events;

using UnityEngine;
using UnityEngine.Events;

namespace Vocario.EventBasedArchitecture
{
    public class GameEventListener<TParams> : AGameEventListener<TParams> where TParams : struct
    {
        [SerializeField]
        new protected UnityEvent<TParams> _onEventRaised = new UnityEvent<TParams>();
        internal GameEventListener(AGameEvent<TParams> gameEvent, object parent, UnityAction<TParams> onEventRaised) : base(gameEvent, parent)
        {
            if (Application.isPlaying)
            {
                _onEventRaised.AddListener(onEventRaised);
            }
            else
            {
                UnityEventTools.AddPersistentListener(_onEventRaised, onEventRaised);
            }
        }

        public override void RaiseEvent(TParams param) => _onEventRaised?.Invoke(param);
    }

    public class GameEventListener : AGameEventListener<GameEventListener.DefaultParams>
    {
        public struct DefaultParams { }
        [SerializeField]
        new protected UnityEvent _onEventRaised = new UnityEvent();
        internal GameEventListener(AGameEvent gameEvent, object parent, UnityAction onEventRaised) : base(gameEvent, parent)
        {
            if (Application.isPlaying)
            {
                _onEventRaised.AddListener(onEventRaised);
            }
            else
            {
                UnityEventTools.AddPersistentListener(_onEventRaised, onEventRaised);
            }
        }

        public override void RaiseEvent(DefaultParams param = default) => _onEventRaised?.Invoke();
    }
}
