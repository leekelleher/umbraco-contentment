/* This Source Code has been copied from Lee Kelleher's Umbraco Polyfill library.
* https://github.com/umbraco/Umbraco-CMS/blob/release-11.0.0/src/Umbraco.Core/Deploy/IContextCache.cs
* Modified under the permissions of the MIT License.
* Modifications are licensed under the Mozilla Public License.
* Copyright © 2023 Lee Kelleher.
* This Source Code Form is subject to the terms of the Mozilla Public
* License, v. 2.0. If a copy of the MPL was not distributed with this
* file, You can obtain one at https://mozilla.org/MPL/2.0/. */

#if NET7_0_OR_GREATER == false
using System;

#if NET472
namespace Umbraco.Core.Deploy
#else
namespace Umbraco.Cms.Core.Deploy
#endif
{
    internal interface IContextCache
    {
        void Clear();
        void Create<T>(string key, T item);
        T GetOrCreate<T>(string key, Func<T> factory);
    }
}
#endif
