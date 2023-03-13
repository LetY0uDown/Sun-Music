using System;

namespace Desktop_Client.Core.Tools.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false)]
internal class SingletonAttribute : Attribute { }