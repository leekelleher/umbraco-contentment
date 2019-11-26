/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Umbraco.Core.Composing;
using Umbraco.Web.Editors;

namespace Umbraco.Community.Contentment.Experience
{
    public sealed class DisablePreviewComponent : IComponent
    {
        public void Initialize()
        {
            EditorModelEventManager.SendingContentModel += (sender, e) =>
            {
                e.Model.AllowPreview = false;
            };
        }

        public void Terminate()
        { }
    }
}
