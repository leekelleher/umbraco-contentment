// SPDX-License-Identifier: MIT
// Copyright © 2025 Lee Kelleher

using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Community.Contentment.Api.Management;
using Umbraco.Community.Contentment.DataEditors;

namespace Umbraco.Community.Contentment.Notifications
{
    internal class ContentmentDataTypeNotificationHandler : INotificationHandler<DataTypeSavedNotification>, INotificationHandler<DataTypeDeletedNotification>
    {
        public void Handle(DataTypeSavedNotification notification)
        {
            foreach (var entity in notification.SavedEntities)
            {
                DataPickerController.ClearCache(entity.Key);
                InputListValueConverter.ClearCache(entity);
            }
        }

        public void Handle(DataTypeDeletedNotification notification)
        {
            foreach (var entity in notification.DeletedEntities)
            {
                DataPickerController.ClearCache(entity.Key);
                InputListValueConverter.ClearCache(entity);
            }
        }
    }
}
