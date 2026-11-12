// SPDX-License-Identifier: MIT
// Copyright © 2026 Lee Kelleher

using Umbraco.Cms.Core.Search.Indexing;

namespace Umbraco.Extensions;

internal static class IndexValueExtensions
{
    /// <summary>
    /// Combines index values into one, or returns <c>null</c> when none of them carry any data.
    /// </summary>
    public static IndexValue? Merge(this IEnumerable<IndexValue> values)
    {
        List<string>? textsR1 = null, textsR2 = null, textsR3 = null, texts = null, keywords = null;
        List<int>? integers = null;
        List<decimal>? decimals = null;
        List<DateTimeOffset>? dates = null;

        foreach (var value in values)
        {
            Add(ref textsR1, value.TextsR1);
            Add(ref textsR2, value.TextsR2);
            Add(ref textsR3, value.TextsR3);
            Add(ref texts, value.Texts);
            Add(ref keywords, value.Keywords);
            Add(ref integers, value.Integers);
            Add(ref decimals, value.Decimals);
            Add(ref dates, value.DateTimeOffsets);
        }

        if (textsR1 is null && textsR2 is null && textsR3 is null && texts is null && keywords is null && integers is null && decimals is null && dates is null)
        {
            return null;
        }

        return new IndexValue
        {
            TextsR1 = textsR1,
            TextsR2 = textsR2,
            TextsR3 = textsR3,
            Texts = texts,
            Keywords = keywords,
            Integers = integers,
            Decimals = decimals,
            DateTimeOffsets = dates,
        };
    }

    private static void Add<T>(ref List<T>? target, IEnumerable<T>? source)
    {
        if (source is null)
        {
            return;
        }

        var items = source.ToList();
        if (items.Count > 0)
        {
            (target ??= []).AddRange(items);
        }
    }
}
