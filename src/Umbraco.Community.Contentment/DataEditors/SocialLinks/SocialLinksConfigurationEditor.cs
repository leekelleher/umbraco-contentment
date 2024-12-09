/* Copyright © 2022 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Text.Json.Nodes;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Extensions;

namespace Umbraco.Community.Contentment.DataEditors
{
    internal sealed class SocialLinksConfigurationEditor : ConfigurationEditor
    {
        // TODO: [LK:2024-12-08] Check if `ToConfigurationEditor` is still being called/used by the backoffice.
        public override IDictionary<string, object> FromConfigurationEditor(IDictionary<string, object> configuration)
        {
            if (configuration.TryGetValueAs("networks", out JsonArray? networks) == true && networks?.Count > 0)
            {
                foreach (JsonObject network in networks)
                {
                    var networkValue = network?.GetValueAs("value", default(JsonObject));
                    if (networkValue?.ContainsKey("icon") == true)
                    {
                        var icon = networkValue.GetValueAs("icon", string.Empty);
                        if (string.IsNullOrWhiteSpace(icon) == false && icon.InvariantContains(" color-") == true)
                        {
                            networkValue["icon"] = icon.Split([' '])[0];
                        }
                    }
                }
            }

            return base.FromConfigurationEditor(configuration);
        }
    }
}
