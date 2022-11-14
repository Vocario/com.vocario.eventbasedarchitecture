// Copyright (C) 2022 Alejandro Güereca

using UnityEngine;
using NUnit.Framework;

namespace Vocario.EventBasedArchitecture.Tests
{
    [TestFixture]
    public class GameEvent
    {
        private EventBasedArchitecture.GameEvent _gameEvent = null;

        [SetUp]
        public void SetUp() => _gameEvent = new EventBasedArchitecture.GameEvent("Test");

        [Test]
        public void AddListenerSuccess()
        {
            var listener = new GameEventListener(_gameEvent, () => Debug.Log($"Game listener raised 1"));
            bool added = _gameEvent.Register(listener);
            Assert.IsTrue(added);
        }

        [Test]
        public void AddListenerFail()
        {
            var listener = new GameEventListener(_gameEvent, () => Debug.Log($"Game listener raised 1"));
            _ = _gameEvent.Register(listener);
            bool added = _gameEvent.Register(listener);
            Assert.IsFalse(added);
        }

        [Test]
        public void Invoke()
        {
            bool calledFirst = false;
            bool calledSecond = false;
            _ = _gameEvent.Register(new GameEventListener(_gameEvent, () => calledFirst = true));
            _ = _gameEvent.Register(new GameEventListener(_gameEvent, () => calledSecond = true));

            _gameEvent.Invoke();
            Assert.IsTrue(calledFirst);
            Assert.IsTrue(calledSecond);
        }
    }
}
