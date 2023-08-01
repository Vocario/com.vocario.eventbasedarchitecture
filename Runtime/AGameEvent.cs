// Copyright (C) 2022 Alejandro Güereca

using System.Linq;
using System;
using UnityEngine;

namespace Vocario.EventBasedArchitecture
{
    [Serializable]
    public abstract class AGameEvent
    {
        [SerializeField]
        private string _name;
        public string Name => _name;

        public AGameEvent() => _name = GetType().ToString();

        [Serializable]
        protected class GameEventListenerDictionary : SerializableDictionary<int, AGameEventListener> { }

        [SerializeField]
        protected GameEventListenerDictionary _gameEventListeners = new GameEventListenerDictionary();

        internal bool Register(AGameEventListener gameEventListener)
        {
            if (_gameEventListeners.Contains(gameEventListener))
            {
                return false;
            }
            _gameEventListeners.Add(gameEventListener.GetHashCode(), gameEventListener);
            return true;
        }

        internal bool Deregister(AGameEventListener gameEventListener) => _gameEventListeners.Remove(gameEventListener.GetHashCode());

        internal void DeregisterAll() => _gameEventListeners.Clear();

        internal void Invoke()
        {
            foreach (AGameEventListener gameEventListener in _gameEventListeners.Values.Reverse<AGameEventListener>())
            {
                gameEventListener.RaiseEvent();
            }
        }
    }

    [Serializable]
    public abstract class AGameEvent<TParams> : AGameEvent where TParams : struct
    {

        internal void Invoke(TParams param)
        {
            foreach (AGameEventListener gameEventListener in _gameEventListeners.Values.Reverse<AGameEventListener>())
            {
                (gameEventListener as AGameEventListener<TParams>).RaiseEvent(param);
            }
        }
    }

}
