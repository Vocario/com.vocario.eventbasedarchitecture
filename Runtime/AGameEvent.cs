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
    }

    [Serializable]
    public abstract class AGameEvent<TParams> : AGameEvent where TParams : struct
    {
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
            Debug.Log($"Code: {gameEventListener.GetHashCode()}");
            _gameEventListeners.Add(gameEventListener.GetHashCode(), gameEventListener);
            return true;
        }

        internal bool Deregister(AGameEventListener gameEventListener)
        {
            Debug.Log($"Code: {gameEventListener.GetHashCode()}");
            return _gameEventListeners.Remove(gameEventListener.GetHashCode());
        }

        internal void DeregisterAll() => _gameEventListeners.Clear();

        internal void Invoke(TParams param)
        {
            foreach (AGameEventListener gameEventListener in _gameEventListeners.Values.Reverse<AGameEventListener>())
            {
                (gameEventListener as AGameEventListener<TParams>).RaiseEvent(param);
            }
        }
    }

}
