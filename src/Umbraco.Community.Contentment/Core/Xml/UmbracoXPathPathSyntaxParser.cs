/* Copyright © 2020 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Reflection;

namespace Umbraco.Core.Xml
{
    // NOTE: Bah! `UmbracoXPathPathSyntaxParser` is marked as internal! It's either copy code, or reflection - here we go!
    // https://github.com/umbraco/Umbraco-CMS/blob/release-8.6.1/src/Umbraco.Core/Xml/UmbracoXPathPathSyntaxParser.cs#L11
    internal class UmbracoXPathPathSyntaxParser
    {
        public static string ParseXPathQuery(
            string xpathExpression,
            int? nodeContextId,
            Func<int, IEnumerable<string>> getPath,
            Func<int, bool> publishedContentExists)
        {
            try
            {
                var assembly = typeof(XPathVariable).Assembly;
                var type = assembly.GetType("Umbraco.Core.Xml.UmbracoXPathPathSyntaxParser");
                var method = type.GetMethod(nameof(ParseXPathQuery), BindingFlags.Static | BindingFlags.Public);
                return method.Invoke(null, new object[] { xpathExpression, nodeContextId, getPath, publishedContentExists }) as string;
            }
            catch { /* ಠ_ಠ */ }

            return xpathExpression;
        }
    }
}
