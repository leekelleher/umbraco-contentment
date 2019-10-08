/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Umbraco.Community.Contentment.DataEditors;
using Umbraco.Core;
using Umbraco.Core.Composing;
using Umbraco.Web.Editors;

namespace Umbraco.Community.Contentment.Experience
{
    public sealed class HidePropertyLabelComponent : IComponent
    {
        public void Initialize()
        {
            EditorModelEventManager.SendingContentModel += (sender, e) =>
            {
                foreach (var variant in e.Model.Variants)
                {
                    foreach (var tab in variant.Tabs)
                    {
                        foreach (var property in tab.Properties)
                        {
                            if (property.Config.ContainsKey(HideLabelConfigurationField.HideLabelAlias))
                            {
                                property.HideLabel = property.Config[HideLabelConfigurationField.HideLabelAlias].TryConvertTo<bool>().Result;
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
