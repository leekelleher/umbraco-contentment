///* Copyright © 2023 Lee Kelleher.
// * This Source Code Form is subject to the terms of the Mozilla Public
// * License, v. 2.0. If a copy of the MPL was not distributed with this
// * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

#if NET6_0 || NET7_0
namespace Umbraco.Cms.Core.Deploy
{
    internal static class ImageSourceParserExtensions
    {
        public static string? ToArtifact(this IImageSourceParser parser, string? value, ICollection<Udi> dependencies, IContextCache contextCache)
             => parser.ToArtifact(value, dependencies);

        public static string? FromArtifact(this IImageSourceParser parser, string? value, IContextCache contextCache)
            => parser.FromArtifact(value);
    }
}
#endif
