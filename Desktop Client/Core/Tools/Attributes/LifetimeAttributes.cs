﻿using System;

namespace Desktop_Client.Core.Tools.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
internal class SingletonAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
internal class TransientAttribute : Attribute { }