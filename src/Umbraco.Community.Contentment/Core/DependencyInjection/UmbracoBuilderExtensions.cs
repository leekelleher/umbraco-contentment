/* Copyright © 2023 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

#if NET472 == false
using System;
using Umbraco.Cms.Core.DependencyInjection;

namespace Umbraco.Extensions
{
    public static partial class UmbracoBuilderExtensions
    {
        public static IUmbracoBuilder AddUmbracoConfiguration(this IUmbracoBuilder builder, Action<IUmbracoBuilder> action)
        {
            action?.Invoke(builder);

            return builder;
        }
    }
}
#endif
