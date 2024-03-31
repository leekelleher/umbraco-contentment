/* Copyright © 2023 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Community.Contentment.DataEditors;

namespace Umbraco.Community.Contentment.Notifications
{
    internal class ContentmentDataTypeNotificationHandler : INotificationHandler<DataTypeSavedNotification>, INotificationHandler<DataTypeDeletedNotification>
    {
        public void Handle(DataTypeSavedNotification notification)
        {
            foreach (var entity in notification.SavedEntities)
            {
                //DataPickerApiController.ClearCache(entity.Key);
            }
        }

        public void Handle(DataTypeDeletedNotification notification)
        {
            foreach (var entity in notification.DeletedEntities)
            {
                //DataPickerApiController.ClearCache(entity.Key);
            }
        }
    }
}
