// Copyright (C) 2022 Alejandro Güereca

using UnityEngine;
using NUnit.Framework;

namespace Vocario.EventBasedArchitecture.Tests
{
    public class TestGameEvent : AGameEvent<TestGameEvent.TestGameEventParams>
    {
        public struct TestGameEventParams
        {
            public int Value;
        }
    }
    [TestFixture]
    internal class GameEventManagerTests
    {
        private GameEventManager _eventManager;


        [SetUp]
        public void SetUp()
        {
            _eventManager = ScriptableObject.CreateInstance<GameEventManager>();
            bool changed = GameEventManager.RefreshEvents();
            Assert.IsTrue(changed);
        }

        [Test]
        public void RaiseEventSuccess()
        {
            bool changed = false;
            _ = GameEventManager.AddListener<TestGameEvent, TestGameEvent.TestGameEventParams>(this, (param) => changed = param.Value == 5);
            TestGameEvent.TestGameEventParams param;
            param.Value = 5;
            _ = GameEventManager.RaiseEvent<TestGameEvent, TestGameEvent.TestGameEventParams>(param);
            Assert.IsTrue(changed);
        }

        [Test]
        public void AddListenerSuccess()
        {
            bool added = GameEventManager.AddListener<TestGameEvent, TestGameEvent.TestGameEventParams>(this, null);
            Assert.IsTrue(added);
        }

        [Test]
        public void RemoveListenerSuccess()
        {
            bool changed = false;
            void handler(TestGameEvent.TestGameEventParams param)
            {
                changed = param.Value == 5;
            }

            _ = GameEventManager.AddListener<TestGameEvent, TestGameEvent.TestGameEventParams>(this, handler);
            bool removed = GameEventManager.RemoveListener<TestGameEvent, TestGameEvent.TestGameEventParams>(this, handler);
            TestGameEvent.TestGameEventParams param;
            param.Value = 5;
            _ = GameEventManager.RaiseEvent<TestGameEvent, TestGameEvent.TestGameEventParams>(param);
            Assert.IsTrue(!changed);
            Assert.IsTrue(removed);
        }

        [Test]
        public void RemoveAllListenersSuccess()
        {
            bool changed = false;
            _ = GameEventManager.AddListener<TestGameEvent, TestGameEvent.TestGameEventParams>(this, (param) => changed = param.Value == 5);
            bool removedAll = GameEventManager.RemoveAllListeners<TestGameEvent, TestGameEvent.TestGameEventParams>();
            TestGameEvent.TestGameEventParams param;
            param.Value = 5;
            _ = GameEventManager.RaiseEvent<TestGameEvent, TestGameEvent.TestGameEventParams>(param);
            Assert.IsTrue(!changed);
            Assert.IsTrue(removedAll);
        }
    }
}
