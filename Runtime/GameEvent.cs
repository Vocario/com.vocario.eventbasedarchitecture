// Copyright (C) 2022 Alejandro Güereca

using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

namespace Vocario.EventBasedArchitecture
{
    [Serializable]
    public class GameEvent
    {
        [SerializeField]
        private string _name;

        public GameEvent(string name)
        {
            _gameEventListeners = new List<AGameEventListener>();
            _name = name;
        }

        //TODO Change to serializable hashset
        [SerializeReference]
        private List<AGameEventListener> _gameEventListeners = new List<AGameEventListener>();


        public bool Register(AGameEventListener gameEventListener)
        {
            if (_gameEventListeners.Contains(gameEventListener))
            {
                return false;
            }
            _gameEventListeners.Add(gameEventListener);
            return true;
        }

        public bool Deregister(AGameEventListener gameEventListener) => _gameEventListeners.Remove(gameEventListener);

        public void Invoke()
        {
            foreach (AGameEventListener gameEventListener in _gameEventListeners.Reverse<AGameEventListener>())
            {
                gameEventListener.RaiseEvent();
            }
        }
    }
}
