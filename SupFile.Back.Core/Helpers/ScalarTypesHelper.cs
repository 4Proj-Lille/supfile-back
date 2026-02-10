using System.Reflection;

namespace SupFile.Back.Core.Helpers;

public static class ScalarTypeHelper
{
    private static readonly Type[] ScalarTypes = new[]
    {
        typeof(string), typeof(int), typeof(long), typeof(short), typeof(byte), typeof(bool), typeof(decimal),
        typeof(double), typeof(float), typeof(DateTime), typeof(DateTimeOffset), typeof(Guid), typeof(Enum)
    };

    public static bool IsScalarProperty(PropertyInfo prop)
    {
        if (string.Equals(prop.Name, "Id", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        var propType = prop.PropertyType;
        if (ScalarTypes.Contains(propType) || propType.IsEnum ||
            (Nullable.GetUnderlyingType(propType)?.IsEnum ?? false))
        {
            return true;
        }

        var underlyingType = Nullable.GetUnderlyingType(propType);
        if (underlyingType != null && ScalarTypes.Contains(underlyingType))
        {
            return true;
        }

        return false;
    }
}
