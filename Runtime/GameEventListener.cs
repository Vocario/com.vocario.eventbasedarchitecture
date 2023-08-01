// Copyright (C) 2022 Alejandro Güereca

using System;

namespace Vocario.EventBasedArchitecture
{
    public class GameEventListener<TParams> : AGameEventListener<TParams> where TParams : struct
    {
        protected object _parent;
        protected Action<TParams> _onEventRaised;

        public GameEventListener(AGameEvent<TParams> gameEvent, object parent, Action<TParams> onEventRaised) : base(gameEvent)
        {
            _parent = parent;
            _onEventRaised = onEventRaised;
        }

        public override void RaiseEvent(TParams param) => _onEventRaised?.Invoke(param);

        public override int GetHashCode() => HashCode.Combine(_parent, _onEventRaised);
    }
}
