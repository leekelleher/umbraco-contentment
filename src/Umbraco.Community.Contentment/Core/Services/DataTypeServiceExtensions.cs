/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using Umbraco.Core.Models;

namespace Umbraco.Core.Services
{
    internal static class DataTypeServiceExtensions
    {
        public static Attempt<string> Copy(this IDataTypeService service, int id, int parentId)
        {
            var dataType = service.GetDataType(id);

            var parent = parentId > 0
                ? service.GetContainer(parentId)
                : default;

            return Copy(service, dataType, parent);
        }

        public static Attempt<string> Copy(this IDataTypeService service, IDataType dataType, EntityContainer parent)
        {
            if (dataType == null)
            {
                return Attempt.Fail(string.Empty, new NullReferenceException("Data Type Not Found"));
            }

            var clone = (IDataType)dataType.DeepClone();

            clone.Id = 0;
            clone.Key = Guid.Empty;

            if (parent != null)
            {
                clone.SetParent(parent);
            }

            service.Save(clone);

            return Attempt.Succeed(clone.Path);
        }
    }
}
