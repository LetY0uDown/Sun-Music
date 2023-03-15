using Desktop_Client.Core.Tools.Attributes;
using System;
using System.Linq;

namespace Desktop_Client.Core.Tools;

internal static class PropertiesSetter
{
    internal static void SetParameters(object obj, params (string Name, object Value)[] objects)
    {
        var classType = obj.GetType();

        var attributes = classType.GetCustomAttributes(false);

        var paramAttributes = attributes.Where(a => a is ParameterAttribute).Cast<ParameterAttribute>();

        if (!paramAttributes.Any())
        {
            throw new InvalidOperationException("Class does not have Parameters");
        }

        var props = classType.GetProperties().Where(p => p.CanWrite);

        if (!props.Any())
        {
            throw new InvalidOperationException("No public properties with accesible set method");
        }

        // TODO: Check is there is multiple attributes with same type and name
        foreach (var a in paramAttributes)
        {
            var propInfo = props.Where(p => p.Name == a.Name && p.PropertyType == a.ParamType).FirstOrDefault();

            if (propInfo is null)
            {
                // TODO: Throw real excepton
            }

            var (name, value) = objects.Where(o => o.Name == a.Name && o.Value.GetType() == a.ParamType).FirstOrDefault();

            propInfo.SetValue(obj, value);
        }
    }
}