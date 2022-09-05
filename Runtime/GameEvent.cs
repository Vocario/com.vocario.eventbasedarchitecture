// Copyright (C) 2022 Alejandro Güereca

using System.Collections.Generic;
using System.Linq;
using System;

namespace Vocario.EventBasedArchitecture
{
    [Serializable]
    public class GameEvent
    {
        private HashSet<AGameEventListener> _gameEventListeners = new HashSet<AGameEventListener>();

        public bool Register(AGameEventListener gameEventListener) => _gameEventListeners.Add(gameEventListener);

        public bool Deregister(AGameEventListener gameEventListener) => _gameEventListeners.Remove(gameEventListener);

        public void Invoke()
        {
            foreach (AGameEventListener gameEventListener in _gameEventListeners.Reverse())
            {
                gameEventListener.RaiseEvent();
            }
        }
    }
}
