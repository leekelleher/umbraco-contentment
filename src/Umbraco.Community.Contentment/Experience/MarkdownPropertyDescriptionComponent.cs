/* Copyright © 2016 Lee Kelleher
 * This Source Code has been derived from my own GitHub Gist.
 * https://gist.github.com/leekelleher/be0b93c069c1c40633a826e63aaaf7b1
 * Modified under the permissions of the MIT License.
 * Modifications are licensed under the Mozilla Public License.
 * Copyright © 2019 Lee Kelleher, Umbrella Inc and other contributors.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using HeyRed.MarkdownSharp;
using Umbraco.Core.Composing;
using Umbraco.Web.Editors;

namespace Umbraco.Community.Contentment.Experience
{
    public sealed class MarkdownPropertyDescriptionComponent : IComponent
    {
        public void Initialize()
        {
            EditorModelEventManager.SendingContentModel += (sender, e) =>
            {
                var markdown = new Markdown()
                {
                    DisableHeaders = true,
                    DisableImages = true,
                };

                foreach (var variant in e.Model.Variants)
                {
                    foreach (var tab in variant.Tabs)
                    {
                        foreach (var property in tab.Properties)
                        {
                            if (string.IsNullOrWhiteSpace(property.Description) == false)
                            {
                                property.Description = markdown
                                    .Transform(property.Description)
                                    .Replace("<a href=", "<a target=\"_blank\" href=")
                                    .Replace("</p>\n\n<p>", "</p><p>")
                                ;
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
