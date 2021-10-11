/* Copyright © 2021 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

#if NET472
using System.ComponentModel;
using Umbraco.Core.IO;

namespace Umbraco.Core.Hosting
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public interface IHostingEnvironment
    {
        string MapPathWebRoot(string path);
    }
}

namespace Umbraco.Community.Contentment.Polyfill
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal sealed class ContenmentHostingEnvironment : Core.Hosting.IHostingEnvironment
    {
        public string MapPathWebRoot(string path)
        {
            return IOHelper.MapPath(path);
        }
    }
}
#endif
