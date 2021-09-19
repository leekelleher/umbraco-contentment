/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

#if NET472
using System.Net.Http.Formatting;
using System.Web.Http.ModelBinding;
using Umbraco.Web.Models.Trees;
using Umbraco.Web.Mvc;
using Umbraco.Web.Trees;
using Umbraco.Web.WebApi.Filters;
using UmbConstants = Umbraco.Core.Constants;
#else
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Trees;
using Umbraco.Cms.Web.BackOffice.Trees;
using Umbraco.Cms.Web.Common.Attributes;
using Umbraco.Cms.Web.Common.ModelBinders;
using UmbConstants = Umbraco.Cms.Core.Constants;
#endif

namespace Umbraco.Community.Contentment.Trees
{
#if NET472
    [Tree(
        UmbConstants.Applications.Settings,
        Constants.Internals.TreeAlias,
        IsSingleNodeTree = true,
        TreeGroup = UmbConstants.Trees.Groups.ThirdParty,
        TreeTitle = Constants.Internals.ProjectName,
        TreeUse = TreeUse.Main)]
    [PluginController(Constants.Internals.PluginControllerName)]
    internal sealed class ContentmentTreeController : TreeController
    {
        protected override TreeNode CreateRootNode(FormDataCollection queryStrings)
        {
            var root = base.CreateRootNode(queryStrings);

            root.Icon = "icon-fa fa-cube";
            root.HasChildren = false;
            root.RoutePath = $"{SectionAlias}/{TreeAlias}/index";
            root.MenuUrl = null;

            return root;
        }

        protected override MenuItemCollection GetMenuForNode(string id, [ModelBinder(typeof(HttpQueryStringModelBinder))] FormDataCollection queryStrings) => null;

        protected override TreeNodeCollection GetTreeNodes(string id, [ModelBinder(typeof(HttpQueryStringModelBinder))] FormDataCollection queryStrings) => null;
    }
#else
    [Tree(
        UmbConstants.Applications.Settings,
        Constants.Internals.TreeAlias,
        IsSingleNodeTree = true,
        TreeGroup = UmbConstants.Trees.Groups.ThirdParty,
        TreeTitle = Constants.Internals.ProjectName,
        TreeUse = TreeUse.Main)]
    [PluginController(Constants.Internals.PluginControllerName)]
    public sealed class ContentmentTreeController : TreeController
    {
        public ContentmentTreeController(
            ILocalizedTextService localizedTextService,
            UmbracoApiControllerTypeCollection umbracoApiControllerTypeCollection,
            IEventAggregator eventAggregator)
            : base(localizedTextService, umbracoApiControllerTypeCollection, eventAggregator)
        { }

        protected override ActionResult<TreeNode> CreateRootNode(FormCollection queryStrings)
        {
            var root = base.CreateRootNode(queryStrings);

            root.Value.Icon = "icon-fa fa-cube";
            root.Value.HasChildren = false;
            root.Value.RoutePath = $"{SectionAlias}/{TreeAlias}/index";
            root.Value.MenuUrl = null;

            return root.Value;
        }

        protected override ActionResult<MenuItemCollection> GetMenuForNode(string id, [ModelBinder(typeof(HttpQueryStringModelBinder))] FormCollection queryStrings) => null;

        protected override ActionResult<TreeNodeCollection> GetTreeNodes(string id, [ModelBinder(typeof(HttpQueryStringModelBinder))] FormCollection queryStrings) => null;
    }
#endif
}
