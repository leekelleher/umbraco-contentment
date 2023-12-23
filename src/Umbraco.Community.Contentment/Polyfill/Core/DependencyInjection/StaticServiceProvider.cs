///* Copyright © 2023 Lee Kelleher.
// * This Source Code Form is subject to the terms of the Mozilla Public
// * License, v. 2.0. If a copy of the MPL was not distributed with this
// * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

#if NET6_0 || NET7_0
using System.ComponentModel;
using OldStaticServiceProvider = Umbraco.Cms.Web.Common.DependencyInjection.StaticServiceProvider;

namespace Umbraco.Cms.Core.DependencyInjection
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal static class StaticServiceProvider
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static IServiceProvider Instance { get; set; } = OldStaticServiceProvider.Instance;
    }
}
#endif
