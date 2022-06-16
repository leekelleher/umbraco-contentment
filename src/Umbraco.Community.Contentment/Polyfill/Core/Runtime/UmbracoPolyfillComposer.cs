/* This Source Code has been copied from Lee Kelleher's Umbraco Polyfill library.
 * https://github.com/leekelleher/umbraco-polyfill/blob/main/src/Core/Runtime/UmbracoPolyfillComposer.cs
 * Modified under the permissions of the MIT License.
 * Modifications are licensed under the Mozilla Public License.
 * Copyright © 2022 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

#if NET472
using Microsoft.AspNetCore.Hosting;
using Umbraco.Core.Composing;
using Umbraco.Core.Hosting;
using Umbraco.Core.IO;
using Umbraco.Core.Logging;
using Umbraco.Core.Serialization;

namespace Umbraco.Core.Runtime
{
    [ComposeAfter(typeof(CoreInitialComposer))]
    [RuntimeLevel(MinLevel = RuntimeLevel.Boot)]
    internal sealed class UmbracoPolyfillComposer : IUserComposer
    {
        public void Compose(Composition composition)
        {
            composition.RegisterAuto(typeof(ILogger<>), typeof(UmbracoPolyfillLogger<>));

            composition.RegisterUnique<IHostingEnvironment, AspNetFxHostingEnvironment>();

            composition.RegisterUnique<IWebHostEnvironment, AspNetFxWebHostEnvironment>();

            composition.RegisterUnique<IIOHelper, IOHelperPolyfill>();

            composition.RegisterUnique<IJsonSerializer, UmbracoPolyfillJsonSerializer>();

            composition.RegisterUnique<IConfigurationEditorJsonSerializer, ConfigurationEditorJsonSerializer>();
        }
    }
}
#endif
