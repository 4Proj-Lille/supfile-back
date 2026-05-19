// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using SupFile.Back.Core.Enums;
using SupFile.Back.Core.Helpers;

namespace SupFile.Back.Core.Dto;

public class SearchQuery
{
    public string? Name { get; set; }
    public string? Extension { get; set; }
    public MediaType? Type { get; set; }
    public DateTime? ModifiedAfter { get; set; }
    public DateTime? ModifiedBefore { get; set; }

    public OrderByType OrderBy { get; set; } = OrderByType.Name;
    
    public string ToGridifyMediaFilter()
    {
        var filters = new List<string>();

        if (!string.IsNullOrWhiteSpace(Name))
            filters.Add($"Name=*{Name}");

        if (!string.IsNullOrWhiteSpace(Extension))
            filters.Add($"Extension=*{Extension.ToLower()}");

        if (Type.HasValue)
        {
            if (Type.Value == MediaType.Other)
            {
                var knownExtensions = Enum.GetValues<MediaType>()
                    .Where(t => t != MediaType.Other)
                    .SelectMany(MediaTypeHelper.GetExtensionsByType);
                var typeFilter = string.Join(",", knownExtensions.Select(ext => $"Extension!={ext}"));
                if (!string.IsNullOrWhiteSpace(typeFilter))
                    filters.Add($"({typeFilter})");
            }
            else
            {
                var extensions = MediaTypeHelper.GetExtensionsByType(Type.Value);
                var typeFilter = string.Join("|", extensions.Select(ext => $"Extension={ext}"));
                if (!string.IsNullOrWhiteSpace(typeFilter))
                    filters.Add($"({typeFilter})");
            }
        }

        if (ModifiedAfter.HasValue)
            filters.Add($"UpdatedDate>={ModifiedAfter.Value:yyyy-MM-dd}");

        if (ModifiedBefore.HasValue)
            filters.Add($"UpdatedDate<={ModifiedBefore.Value:yyyy-MM-dd}");

        return string.Join(",", filters);
    }
    
    public string ToGridifyMediaOrderBy() => OrderBy switch
    {
        OrderByType.Name => "Name asc",
        OrderByType.Date => "UpdatedDate desc",
        OrderByType.Size => "Size desc",
        OrderByType.Type => "Extension asc",
        _                => "Name asc"
    };
    
    public string ToGridifyFolderFilter()
    {
        var filters = new List<string>();

        if (!string.IsNullOrWhiteSpace(Name))
            filters.Add($"Name=*{Name}");

        if (ModifiedAfter.HasValue)
            filters.Add($"CreatedDate>={ModifiedAfter.Value:yyyy-MM-dd}");

        if (ModifiedBefore.HasValue)
            filters.Add($"CreatedDate<={ModifiedBefore.Value:yyyy-MM-dd}");

        return string.Join(",", filters);
    }

    public string ToGridifyFolderOrderBy() => OrderBy switch
    {
        OrderByType.Date => "UpdatedDate desc",
        _                => "Name asc"
    };
}
