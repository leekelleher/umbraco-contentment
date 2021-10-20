/* Copyright © 2021 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

#if NET472
using System.ComponentModel;

namespace Umbraco.Core.IO
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public interface IIOHelper
    {
        string ResolveRelativeOrVirtualUrl(string path);
    }
}

namespace Umbraco.Community.Contentment.Polyfill
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal sealed class ContenmentIOHelper : Core.IO.IIOHelper
    {
        public string ResolveRelativeOrVirtualUrl(string path)
        {
            return Core.IO.IOHelper.ResolveUrl(path);
        }
    }
}
#endif
