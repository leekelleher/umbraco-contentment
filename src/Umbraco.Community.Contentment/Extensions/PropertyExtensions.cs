// SPDX-License-Identifier: MIT
// Copyright © 2026 Lee Kelleher

using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Serialization;

namespace Umbraco.Extensions;

internal static class PropertyExtensions
{
    /// <summary>
    /// Deserializes the property's stored JSON value, or returns <c>null</c> when there is no JSON value.
    /// </summary>
    public static T? GetJsonValue<T>(this IProperty property, string? culture, string? segment, bool published, IJsonSerializer jsonSerializer)
        where T : class
        => property.GetValue(culture, segment, published) is string value &&
            string.IsNullOrWhiteSpace(value) is false &&
            value.DetectIsJson() is true
                ? jsonSerializer.Deserialize<T>(value)
                : null;
}
