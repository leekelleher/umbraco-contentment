///* Copyright © 2023 Lee Kelleher.
// * This Source Code Form is subject to the terms of the Mozilla Public
// * License, v. 2.0. If a copy of the MPL was not distributed with this
// * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

#if NET8_0_OR_GREATER == false
using System.Collections.Generic;

#if NET472
namespace Umbraco.Core.Deploy
#else
namespace Umbraco.Cms.Core.Deploy
#endif
{
    internal static class LocalLinkParserExtensions
    {
        public static string ToArtifact(this ILocalLinkParser parser, string value, ICollection<Udi> dependencies, IContextCache contextCache)
             => parser.ToArtifact(value, dependencies);

        public static string FromArtifact(this ILocalLinkParser parser, string value, IContextCache contextCache)
            => parser.FromArtifact(value);
    }
}
#endif
