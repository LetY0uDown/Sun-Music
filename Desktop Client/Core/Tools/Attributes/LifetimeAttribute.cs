using System;

namespace Desktop_Client.Core.Tools.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class LifetimeAttribute : Attribute
{
    public LifetimeAttribute(Lifetime lifetime)
    {
        Lifetime = lifetime;
    }

    public Lifetime Lifetime { get; private init; }
}

public enum Lifetime
{
    Singleton,
    Transient
}