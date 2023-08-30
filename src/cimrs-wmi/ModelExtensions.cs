namespace CimRs.Wmi;

using CimRs.Wmi.Controllers;
using Microsoft.Management.Infrastructure;

public static class ModelExtensions
{
    public static CimResource ToModel(this CimInstance instance)
    {
        var keyProperty = instance.CimInstanceProperties.First(p => p.Flags.HasFlag(CimFlags.Key));

        var cim = new CimResource
        {
            Kind = "instance",
            Class = instance.CimSystemProperties.ClassName,
            Self = instance.GetSelf(keyProperty.Value),
            Properties = instance.CimInstanceProperties.ToDictionary(p => p.Name, p => p.Value),
        };

        return cim;
    }

    public static Models.CimClass ToModel(this CimClass cimClass)
        => new()
        {
            Kind = "class",
            Namespace = cimClass.CimSystemProperties.Namespace,
            Name = cimClass.CimSystemProperties.ClassName,
            SuperClassName = cimClass.CimSuperClassName,
            Self = "",
            Properties = cimClass.CimClassProperties.ToDictionary(p => p.Name, ToModel),
            Methods = cimClass.CimClassMethods.ToDictionary(p => p.Name, ToModel),
        };

    private static Models.CimPropertyDefinition ToModel(CimPropertyDeclaration property)
        => new()
        {
            Type = property.CimType.ToString(),
            ClassName = property.ReferenceClassName,
            DefaultValue = property.Value,
            Qualifiers = property.Qualifiers.ToDictionary(q => q.Name, ToModel),
        };

    private static Models.CimMethodDefinition ToModel(CimMethodDeclaration method)
        => new()
        {
            Type = method.ReturnType.ToString(),
            Qualifiers = method.Qualifiers.ToDictionary(q => q.Name, ToModel),
        };

    private static Models.CimQualifier ToModel(CimQualifier qualifier)
        => new()
        {
            Value = qualifier.Value,
        };

    private static string GetSelf(this CimInstance cimInstance, object id)
    {
        var keyProperties = cimInstance.CimInstanceProperties.Where(p => p.Flags.HasFlag(CimFlags.Key));
        var instanceId = string.Join(',', keyProperties.Select(p => p.Name + "=" + p.Value));

        return $"{CimController.Prefix}/{Uri.EscapeDataString(cimInstance.CimSystemProperties.Namespace)}/classes/{cimInstance.CimSystemProperties.ClassName}/instances/{instanceId}";
    }

    public static object FromString(this CimType cimType, string value)
    {
        switch (cimType)
        {
            case CimType.Boolean:
                return bool.Parse(value);
            case CimType.BooleanArray:
                return new[] { bool.Parse(value) };
            case CimType.Char16:
                return char.Parse(value);
            case CimType.Char16Array:
                return new[] { char.Parse(value) };
            case CimType.DateTime:
                return DateTime.Parse(value);
            case CimType.DateTimeArray:
                return new[] { DateTime.Parse(value) };
            case CimType.Instance:
            case CimType.InstanceArray:
                throw new NotImplementedException();
            case CimType.Real32:
                return float.Parse(value);
            case CimType.Real32Array:
                return new[] { float.Parse(value) };
            case CimType.Real64:
                return double.Parse(value);
            case CimType.Real64Array:
                return new[] { double.Parse(value) };
            case CimType.Reference:
            case CimType.ReferenceArray:
                throw new NotImplementedException();
            case CimType.SInt16:
                return short.Parse(value);
            case CimType.SInt16Array:
                return new[] { short.Parse(value) };
            case CimType.SInt32:
                return int.Parse(value);
            case CimType.SInt32Array:
                return new[] { int.Parse(value) };
            case CimType.SInt64:
                return long.Parse(value);
            case CimType.SInt64Array:
                return new[] { long.Parse(value) };
            case CimType.SInt8:
                return sbyte.Parse(value);
            case CimType.SInt8Array:
                return new[] { sbyte.Parse(value) };
            case CimType.String:
                return value;
            case CimType.StringArray:
                return new[] { value };
            case CimType.UInt16:
                return ushort.Parse(value);
            case CimType.UInt16Array:
                return new[] { ushort.Parse(value) };
            case CimType.UInt32:
                return uint.Parse(value);
            case CimType.UInt32Array:
                return new[] { uint.Parse(value) };
            case CimType.UInt64:
                return ulong.Parse(value);
            case CimType.UInt64Array:
                return new[] { ulong.Parse(value) };
            case CimType.UInt8:
                return byte.Parse(value);
            case CimType.UInt8Array:
                return new[] { byte.Parse(value) };
            default:
                throw new ArgumentException(nameof(cimType));
        }
    }
}