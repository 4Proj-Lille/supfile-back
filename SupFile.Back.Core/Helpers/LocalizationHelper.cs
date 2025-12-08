namespace SupFile.Back.Core.Helpers;

/// <summary>
/// Helper class to localize datas
/// </summary>
public static class LocalizationHelper
{
    /// <summary>
    /// Converts an enum to its localized description
    /// </summary>
    /// <param name="value">The enum to convert</param>
    /// <returns>The localized description of the enum</returns>
    public static string ToLocalizedEnum(this Enum? value)
    {
        if (value == null) return string.Empty;

        var fieldInfo = value.GetType().GetField(value.ToString());
        if (fieldInfo == null) return value.ToString();
        
        var attributes = (LocalizedDescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(LocalizedDescriptionAttribute), false);
        return attributes.Length > 0 ? attributes[0].Description : value.ToString();
    }
}
