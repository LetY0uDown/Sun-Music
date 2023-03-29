using System;

namespace Desktop_Client.Core.Tools.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false)]
public class BaseTypeAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class LifetimeAttribute : Attribute
{
    public LifetimeAttribute(Lifetime lifetime)
    {
        Lifetime = lifetime;
    }

    public Lifetime Lifetime { get; }
}

public enum Lifetime
{
    Singleton,
    Transient
}