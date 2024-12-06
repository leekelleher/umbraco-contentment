/* Copyright © 2022 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Newtonsoft.Json.Linq;
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Extensions;

namespace Umbraco.Community.Contentment.DataEditors
{
    internal sealed class SocialLinksConfigurationEditor : ConfigurationEditor
    {
        internal const string OverlayView = "overlayView";
        internal const string Network = "network";
        internal const string Networks = "networks";

        private readonly IIOHelper _ioHelper;

        public SocialLinksConfigurationEditor(IIOHelper ioHelper)
            : base()
        {
            _ioHelper = ioHelper;

            DefaultConfiguration.Add(Networks, new[]
            {
                new { key = Network, value = new { network = "facebook", name = "Facebook", url = "https://facebook.com/", icon = "icon-facebook", backgroundColor = "#3b5998", iconColor = "#fff" } },
                new { key = Network, value = new { network = "x-twitter", name = "X (formerly Twitter)", url = "https://twitter.com/", icon = "icon-x-twitter", backgroundColor = "#000", iconColor = "#fff" } },
                new { key = Network, value = new { network = "instagram", name = "Instagram", url = "https://instagram.com/", icon = "icon-instagram", backgroundColor = "#305777", iconColor = "#fff" } },
                new { key = Network, value = new { network = "linkedin", name = "LinkedIn", url = "https://linkedin.com/in/", icon = "icon-linkedin", backgroundColor = "#007bb6", iconColor = "#fff" } },
                new { key = Network, value = new { network = "mastodon", name = "Mastodon", url = "https://mastodon.social/", icon = "icon-mastodon", backgroundColor = "#5b4be1", iconColor = "#fff" } },
                new { key = Network, value = new { network = "youtube", name = "YouTube", url = "https://youtube.com/", icon = "icon-youtube", backgroundColor = "#f00", iconColor = "#fff" } },
                new { key = Network, value = new { network = "github", name = "GitHub", url = "https://github.com/", icon = "icon-github", backgroundColor = "#000", iconColor = "#fff" } },
                new { key = Network, value = new { network = "discord", name = "Discord", url = "https://discord.com/users/", icon = "icon-discord", backgroundColor = "#404eed", iconColor = "#fff" } },
            });

            var items = new[]
            {
                new ConfigurationEditorModel
                {
                    Key = Network,
                    Name = "Social network",
                    Description = string.Empty,
                    Icon = SocialLinksDataEditor.DataEditorIcon,
                    DefaultValues = new Dictionary<string, object>
                    {
                        { "icon", UmbConstants.Icons.DefaultIcon },
                    },
                    Expressions = new Dictionary<string, string>
                    {
                        { "name", "{{ name }}" },
                        { "description", "{{ url }}" },
                        { "icon", "{{ icon.split(\" \")[0] }}" },
                        { "cardStyle", "{ \"background-color\": \"{{ backgroundColor }}\" }" },
                        { "iconStyle", "{ \"color\": \"{{ iconColor }}\" }" },
                    },
                    Fields = new[]
                    {
                        new ContentmentConfigurationField
                        {
                            Key = Network,
                            Name = nameof(Network),
                            Description = "An alias for the social network. This will be used as the value of the selection.",
                        },
                        new ContentmentConfigurationField
                        {
                            Key = "name",
                            Name = "Name",
                            Description = "This will be used as the label of the social network in selection modal.",
                        },
                        new ContentmentConfigurationField
                        {
                            Key = "url",
                            Name = "Base URL",
                            Description = "This will be the starting part of the social network's profile URL.",
                        },
                        new ContentmentConfigurationField
                        {
                            Key = "icon",
                            Name = "Icon",
                            Description = "Typically select the logo for the social network.",
                            Config = new Dictionary<string, object>
                            {
                                { "hideColors", Constants.Values.True },
                                { "size", "small" },
                            }
                        },
                        new NotesConfigurationField($@"<details class=""alert alert-info"">
<summary>Would you like to use a <strong>custom icon</strong>?</summary>
<p>To add your own custom icons to the Umbraco backoffice, add any SVG icon files to a custom plugin folder, e.g. <code>~/App_Plugins/[YourPluginName]/backoffice/icons/</code>.</p>
<p>For a step-by-step guide, Warren Buckley has a video tutorial: <a href=""https://www.youtube.com/watch?v=m90uxZBVFOw"" target=""_blank""><strong>How to Add Custom SVG icons to Umbraco Icon Picker</strong></a>.</p>
</details>", true),
                        new ContentmentConfigurationField
                        {
                            Key = "backgroundColor",
                            Name = "Background color",
                            Description = "The background color for the icon.",
                        },
                        new ContentmentConfigurationField
                        {
                            Key = "iconColor",
                            Name = "Icon color",
                            Description = "The foreground color of the icon.",
                        },
                    },
                    OverlaySize = OverlaySize.Medium,
                }
            };

            //Fields.Add(new ContentmentConfigurationField
            //{
            //    Key = Networks,
            //    Name = "Social networks",
            //    Description = "Define the icon set for the available social networks.",
            //    View = ioHelper.ResolveRelativeOrVirtualUrl(ConfigurationEditorDataEditor.DataEditorViewPath),
            //    Config = new Dictionary<string, object>
            //    {
            //        { "allowDuplicates", Constants.Values.True },
            //        { Constants.Conventions.ConfigurationFieldAliases.OverlayView, ioHelper.ResolveRelativeOrVirtualUrl(ConfigurationEditorDataEditor.DataEditorOverlayViewPath) ?? string.Empty },
            //        { "displayMode", "cards" },
            //        { Constants.Conventions.ConfigurationFieldAliases.Items, items },
            //        { EnableDevModeConfigurationField.EnableDevMode, Constants.Values.True },
            //    }
            //});

            //Fields.Add(new ContentmentConfigurationField
            //{
            //    Key = "confirmRemoval",
            //    Name = "Confirm removals?",
            //    Description = "Select to enable a confirmation prompt when removing an item.",
            //    View = "boolean",
            //});

            //DefaultConfiguration.Add(MaxItemsConfigurationField.MaxItems, 0);
            //Fields.Add(new MaxItemsConfigurationField(ioHelper));
            //Fields.Add(new EnableDevModeConfigurationField());
        }

        public override IDictionary<string, object> FromConfigurationEditor(IDictionary<string, object> configuration)
        {
            if (configuration.TryGetValueAs(Networks, out JArray? networks) == true && networks?.Count > 0)
            {
                foreach (JObject network in networks)
                {
                    var networkValue = network.GetValueAs("value", default(JObject));
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

        public override IDictionary<string, object> ToValueEditor(IDictionary<string, object> configuration)
        {
            var config = base.ToValueEditor(configuration);

            if (config.TryGetValueAs(Networks, out JArray? array1) && array1?.Count > 0)
            {
                var networks = new List<JObject?>();

                for (var i = 0; i < array1.Count; i++)
                {
                    networks.Add(array1[i]["value"] as JObject);
                }

                config[Networks] = networks;
            }

            if (config.ContainsKey(OverlayView) == false)
            {
                config.Add(OverlayView, _ioHelper.ResolveRelativeOrVirtualUrl(SocialLinksDataEditor.DataEditorOverlayViewPath) ?? string.Empty);
            }

            return config;
        }
    }
}
