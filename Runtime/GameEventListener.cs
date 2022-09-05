// Copyright (C) 2022 Alejandro Güereca

using System;

namespace Vocario.EventBasedArchitecture
{
    public class GameEventListener : AGameEventListener
    {
        protected Action _onEventRaised;

        public GameEventListener(GameEvent gameEvent, Action onEventRaised) : base(gameEvent) => _onEventRaised = onEventRaised;

        public override void RaiseEvent() => _onEventRaised?.Invoke();
    }
}
