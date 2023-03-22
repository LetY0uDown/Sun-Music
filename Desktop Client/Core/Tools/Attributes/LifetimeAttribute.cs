using System;

namespace Desktop_Client.Core.Tools.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
internal class HasLifetimeAttribute : Attribute 
{
    public HasLifetimeAttribute(Lifetime lifetime)
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