/* Copyright © 2020 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */
/* This component has been inspired by Bjørn Fridal's Icon Picker package.
 * <https://our.umbraco.com/packages/backoffice-extensions/icon-picker/> */

using Umbraco.Core;
using Umbraco.Core.Composing;
using Umbraco.Web.Trees;
using UmbApplications = Umbraco.Core.Constants.Applications;

namespace Umbraco.Community.Contentment.Experience
{
    public sealed class SetContentTreeIconComponent : IComponent
    {
        public void Initialize()
        {
            TreeControllerBase.TreeNodesRendering += (sender, e) =>
            {
                if (sender.TreeAlias.InvariantEquals(UmbApplications.Content) == false)
                {
                    return;
                }

                if (e.Nodes.Count > 0)
                {
                    const string propertyTypeAlias = "umbracoIcon";

                    foreach (var node in e.Nodes)
                    {
                        if (node.Id is string nodeId && int.TryParse(nodeId, out var id) && id > 0)
                        {
                            var content = Current.Services.ContentService.GetById(id);
                            if (content.Properties.TryGetValue(propertyTypeAlias, out var property) &&
                                property.GetValue() is string icon &&
                                string.IsNullOrWhiteSpace(icon) == false)
                            {
                                node.Icon = icon;
                            }
                        }
                    }
                }
            };
        }

        public void Terminate()
        { }
    }
}
