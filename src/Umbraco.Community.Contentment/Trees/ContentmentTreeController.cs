/* Copyright © 2019 Lee Kelleher, Umbrella Inc and other contributors.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Net.Http.Formatting;
using System.Web.Http.ModelBinding;
using Umbraco.Web.Models.Trees;
using Umbraco.Web.Mvc;
using Umbraco.Web.Trees;
using Umbraco.Web.WebApi.Filters;
using UmbConstants = Umbraco.Core.Constants;

namespace Umbraco.Community.Contentment.Trees
{
    [Tree(
        UmbConstants.Applications.Settings,
        Constants.Internals.TreeAlias,
        IsSingleNodeTree = true,
        //SortOrder = 1,
        TreeGroup = UmbConstants.Trees.Groups.ThirdParty,
        TreeTitle = Constants.Internals.ProjectName,
        TreeUse = TreeUse.Main)]
    [PluginController(Constants.Internals.PluginControllerName)]
    public class ContentmentTreeController : TreeController
    {
        protected override TreeNode CreateRootNode(FormDataCollection queryStrings)
        {
            var root = base.CreateRootNode(queryStrings);

            root.Icon = "icon-smiley";
            root.HasChildren = false;
            root.RoutePath = $"{UmbConstants.Applications.Settings}/{Constants.Internals.TreeAlias}/index";
            root.MenuUrl = null;

            return root;
        }

        protected override MenuItemCollection GetMenuForNode(string id, [ModelBinder(typeof(HttpQueryStringModelBinder))] FormDataCollection queryStrings)
        {
            return null;
        }

        protected override TreeNodeCollection GetTreeNodes(string id, [ModelBinder(typeof(HttpQueryStringModelBinder))] FormDataCollection queryStrings)
        {
            return null;
        }
    }
}
