// Copyright (C) 2022 Alejandro Güereca

namespace Vocario.EventBasedArchitecture
{
    public abstract class AGameEventListener
    {
        protected GameEvent _gameEvent;
        protected AGameEventListener(GameEvent gameEvent) => _gameEvent = gameEvent;

        public abstract void RaiseEvent();
        public void Deregister() => _gameEvent?.Deregister(this);
    }
}
