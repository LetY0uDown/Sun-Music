using System;
using System.Diagnostics.CodeAnalysis;

namespace Desktop_Client.Core.Tools.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class ParameterAttribute : Attribute
{
    public ParameterAttribute (Type paramType, string name)
    {
        ParamType = paramType;
        Name = name;
    }

    public string Name { get; private init; }

    public Type ParamType { get; private init; }

    public override bool Equals ([NotNullWhen(true)] object obj)
    {
        if (obj is ParameterAttribute second)
            return Name == second.Name && ParamType == second.ParamType;

        return false;
    }

    public override int GetHashCode ()
    {
        return base.GetHashCode();
    }
}
