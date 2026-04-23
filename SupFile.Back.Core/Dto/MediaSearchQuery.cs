// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using SupFile.Back.Core.Enums;

namespace SupFile.Back.Core.Dto;

public class MediaSearchQuery
{
    public string? Name { get; set; }
    public string? Extension { get; set; }
    public MediaType? Type { get; set; }
    public DateTime? ModifiedAfter { get; set; }
    public DateTime? ModifiedBefore { get; set; }

    public string ToGridifyFilter()
    {
        var filters = new List<string>();

        if (!string.IsNullOrWhiteSpace(Name))
            filters.Add($"Name=*{Name.ToLower()}");

        if (!string.IsNullOrWhiteSpace(Extension))
            filters.Add($"Extension={Extension}");

        if (Type.HasValue)
        {
            var extensions = MediaTypeHelper.GetExtensionsByType(Type.Value);
            var typeFilter = string.Join("|", extensions.Select(ext => $"Extension={ext}"));
            if (!string.IsNullOrWhiteSpace(typeFilter))
                filters.Add($"({typeFilter})");
        }

        if (ModifiedAfter.HasValue)
            filters.Add($"CreatedDate>={ModifiedAfter.Value:yyyy-MM-dd}");

        if (ModifiedBefore.HasValue)
            filters.Add($"CreatedDate<={ModifiedBefore.Value:yyyy-MM-dd}");

        return string.Join(",", filters);
    }
}
