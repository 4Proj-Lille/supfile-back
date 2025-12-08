using System.ComponentModel;
using System.Globalization;

namespace SupFile.Back.Core.Attributes;

/// <inheritdoc />
public sealed class LocalizedDescriptionAttribute : DescriptionAttribute
{
    private readonly ResourceManager _resourceManager;

    /// <inheritdoc />
    public LocalizedDescriptionAttribute(string resourceKey, Type resourceSet)
    {
        ResourceKey = resourceKey;
        ResourceSet = resourceSet;
        _resourceManager = new ResourceManager(resourceSet);
    }

    public string ResourceKey { get; }
    public Type ResourceSet { get; }

    /// <inheritdoc />
    public override string Description
    {
        get => string.IsNullOrEmpty(_resourceManager.GetString(ResourceKey, CultureInfo.CurrentCulture))
            ? ResourceKey
            : _resourceManager.GetString(ResourceKey, CultureInfo.CurrentCulture)!;
    }
}
