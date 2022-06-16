/* This Source Code has been copied from Lee Kelleher's Umbraco Polyfill library.
 * https://github.com/leekelleher/umbraco-polyfill/blob/main/src/Core/Composing/CompositionExtensions.cs
 * Modified under the permissions of the MIT License.
 * Modifications are licensed under the Mozilla Public License.
 * Copyright © 2020 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

#if NET472
using System;
using LightInject;

namespace Umbraco.Core.Composing
{
    public static partial class CompositionExtensions
    {
        internal static void RegisterAuto(this Composition composition, Type serviceBaseType, Type implementingBaseType)
        {
            if (composition.Concrete is ServiceContainer container)
            {
                container.RegisterFallback((serviceType, serviceName) =>
                {
                    if (serviceType.IsInterface &&
                        implementingBaseType.IsInterface == false &&
                        serviceType.IsGenericType &&
                        serviceType.GetGenericTypeDefinition() == serviceBaseType)
                    {
                        var genericArgs = serviceType.GetGenericArguments();
                        var implementingType = implementingBaseType.MakeGenericType(genericArgs);

                        container.Register(serviceType, implementingType);
                    }

                    return false;
                }, null);
            }
        }
    }
}
#endif
