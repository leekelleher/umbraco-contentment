/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Umbraco.Community.Contentment.DataEditors;
using Umbraco.Core;
using Umbraco.Core.Composing;
using Umbraco.Web.Runtime;

namespace Umbraco.Community.Contentment.Composing
{
    [ComposeAfter(typeof(WebInitialComposer))]
    [RuntimeLevel(MinLevel = RuntimeLevel.Boot)]
    internal sealed class ContentmentComposer : IUserComposer
    {
        public void Compose(Composition composition)
        {
            composition
                .ContentmentListItems()
                    .Add(() => composition.TypeLoader.GetTypes<IContentmentListItem>())
            ;

            composition.RegisterUnique<ConfigurationEditorUtility>();

            if (composition.RuntimeState.Level > RuntimeLevel.Install)
            {
                composition
                    .Components()
                        .Append<ContentmentComponent>()
                ;
            }

        }
    }
}
