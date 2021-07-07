/* Copyright © 2021 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Microsoft.Extensions.Options;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Trees;
using Umbraco.Cms.Web.BackOffice.Trees;
using Umbraco.Community.Contentment.Trees;

namespace Umbraco.Community.Contentment.Notifications
{
    internal class ContentmentUmbracoApplicationStartingNotification : INotificationHandler<UmbracoApplicationStartingNotification>
    {
        private readonly ContentmentSettings _contentmentSettings;
        private readonly TreeCollection _treeCollection;

        public ContentmentUmbracoApplicationStartingNotification(
            IOptions<ContentmentSettings> contentmentSettings,
            TreeCollection treeCollection)
        {
            _contentmentSettings = contentmentSettings.Value;
            _treeCollection = treeCollection;
        }

        public void Handle(UmbracoApplicationStartingNotification notification)
        {
            if (notification.RuntimeLevel == Cms.Core.RuntimeLevel.Run &&
                _contentmentSettings.DisableTree == true &&
                _treeCollection != null)
            {
                _treeCollection.RemoveTreeController<ContentmentTreeController>();
            }
        }
    }
}
