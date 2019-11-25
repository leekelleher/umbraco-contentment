/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Linq;
using Umbraco.Core;
using Umbraco.Core.Composing;
using Umbraco.Core.IO;
using Umbraco.Core.Services;
using Umbraco.Core.Services.Implement;
using Umbraco.Web.Models.Trees;
using Umbraco.Web.Trees;

namespace Umbraco.Community.Contentment.Experience
{
    public sealed class DataTypeTreeCopyMenuItemComponent : IComponent
    {
        private readonly ILocalizedTextService _textService;

        public DataTypeTreeCopyMenuItemComponent(ILocalizedTextService textService)
        {
            _textService = textService;
        }

        public void Initialize()
        {
            TreeControllerBase.MenuRendering += (sender, args) =>
            {
                if (sender.TreeAlias.InvariantEquals("dataTypes") == false)
                    return;

                if (args.NodeId.InvariantEquals(Core.Constants.System.RootString))
                    return;

                // This is a quicker way to detect if the node is an actual data-type, (not a container).
                if (args.Menu.Items.Any(x => x.Alias.InvariantEquals("move")) == false)
                    return;

                args.Menu.Items.Add(new MenuItem("copy", _textService.Localize("actions/copy"))
                {
                    Icon = "documents",
                    OpensDialog = true,
                    AdditionalData = { { "actionView", IOHelper.ResolveUrl($"{Constants.Internals.BackOfficePathRoot}data-type-copy.html") }, }
                });
            };
        }

        public void Terminate()
        { }
    }
}
