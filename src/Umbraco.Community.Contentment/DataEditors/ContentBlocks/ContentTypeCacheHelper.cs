/* Copyright © 2016 UMCO, Our Umbraco and other contributors.
 * This Source Code has been derived from Inner Content.
 * https://github.com/umco/umbraco-inner-content/blob/2.0.4/src/Our.Umbraco.InnerContent/Helpers/ContentTypeCacheHelper.cs
 * Modified under the permissions of the MIT License.
 * Modifications are licensed under the Mozilla Public License.
 * Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Concurrent;
using Umbraco.Core.Models;
using Umbraco.Core.Services;

namespace Umbraco.Community.Contentment.DataEditors
{
    internal static class ContentTypeCacheHelper
    {
        private static readonly ConcurrentDictionary<Guid, string> _forward = new ConcurrentDictionary<Guid, string>();
        private static readonly ConcurrentDictionary<string, Guid> _reverse = new ConcurrentDictionary<string, Guid>();
        private static readonly ConcurrentDictionary<string, string> _icons = new ConcurrentDictionary<string, string>();

        public static void ClearAll()
        {
            _forward.Clear();
            _reverse.Clear();
            _icons.Clear();
        }

        public static void TryAdd(IContentType contentType)
        {
            TryAdd(contentType.Key, contentType.Alias, contentType.Icon);
        }

        public static void TryAdd(Guid guid, string alias, string icon = null)
        {
            _forward.TryAdd(guid, alias);
            _reverse.TryAdd(alias, guid);
            _icons.TryAdd(alias, icon);
        }

        public static bool TryGetAlias(Guid key, out string alias, IContentTypeService contentTypeService = null)
        {
            if (_forward.TryGetValue(key, out alias))
                return true;

            // The alias isn't cached, we can attempt to get it via the content-type service, using the GUID.
            if (contentTypeService != null)
            {
                var contentType = contentTypeService.Get(key);
                if (contentType != null)
                {
                    TryAdd(contentType);
                    alias = contentType.Alias;
                    return true;
                }
            }

            return false;
        }

        public static bool TryGetIcon(string alias, out string icon, IContentTypeService contentTypeService = null)
        {
            if (_icons.TryGetValue(alias, out icon))
                return true;

            // The icon isn't cached, we can attempt to get it via the content-type service, using the alias.
            if (contentTypeService != null)
            {
                var contentType = contentTypeService.Get(alias);
                if (contentType != null)
                {
                    TryAdd(contentType);
                    icon = contentType.Icon;
                    return true;
                }
            }

            return false;
        }

        public static bool TryGetGuid(string alias, out Guid key, IContentTypeService contentTypeService = null)
        {
            if (_reverse.TryGetValue(alias, out key))
                return true;

            // The GUID isn't cached, we can attempt to get it via the content-type service, using the alias.
            if (contentTypeService != null)
            {
                var contentType = contentTypeService.Get(alias);
                if (contentType != null)
                {
                    TryAdd(contentType);
                    key = contentType.Key;
                    return true;
                }
            }

            return false;
        }

        public static void TryRemove(IContentType contentType)
        {
            if (TryRemove(contentType.Alias) == false)
            {
                TryRemove(contentType.Key);
            }
        }

        public static bool TryRemove(Guid guid)
        {
            return _forward.TryRemove(guid, out var alias)
                ? _reverse.TryRemove(alias, out _)
                : false;
        }

        public static bool TryRemove(string alias)
        {
            _icons.TryRemove(alias, out _);

            return _reverse.TryRemove(alias, out var guid)
                ? _forward.TryRemove(guid, out _)
                : false;
        }
    }
}
