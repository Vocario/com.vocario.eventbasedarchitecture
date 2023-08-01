// Copyright (C) 2022 Alejandro Güereca

using System;

namespace Vocario.EventBasedArchitecture
{
    public class GameEventListener<TParams> : AGameEventListener<TParams> where TParams : struct
    {
        protected object _parent;
        protected Action<TParams> _onEventRaised;

        internal GameEventListener(AGameEvent<TParams> gameEvent, object parent, Action<TParams> onEventRaised) : base(gameEvent)
        {
            _parent = parent;
            _onEventRaised = onEventRaised;
        }

        public override void RaiseEvent(TParams param) => _onEventRaised?.Invoke(param);
        public override void RaiseEvent() => _onEventRaised?.Invoke(default);

        public override int GetHashCode() => HashCode.Combine(_parent, _onEventRaised);
    }

    public class GameEventListener : AGameEventListener
    {
        protected object _parent;
        protected Action _onEventRaised;

        internal GameEventListener(AGameEvent gameEvent, object parent, Action onEventRaised) : base(gameEvent)
        {
            _parent = parent;
            _onEventRaised = onEventRaised;
        }

        public override void RaiseEvent() => _onEventRaised?.Invoke();

        public override int GetHashCode() => HashCode.Combine(_parent, _onEventRaised);
    }
}
