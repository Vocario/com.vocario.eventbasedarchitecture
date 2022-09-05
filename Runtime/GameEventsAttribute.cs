// Copyright (C) 2022 Alejandro Güereca

using System;

namespace Vocario.EventBasedArchitecture
{
    [AttributeUsage(AttributeTargets.Enum, Inherited = false, AllowMultiple = true)]
    public sealed class GameEventsAttribute : Attribute
    {
        public string Category { get; }
        public GameEventsAttribute(string category = "Default") => Category = category;
    }
}
