/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Umbraco.Cms.Infrastructure.Migrations;

namespace Umbraco.Community.Contentment.Migrations
{
    internal sealed class RegisterUmbracoPackageEntry : MigrationBase
    {
        public const string State = "{contentment-reg-umb-pkg-entry}";

        public RegisterUmbracoPackageEntry(IMigrationContext context)
            : base(context)
        { }

        protected override void Migrate()
        {
            // NOTE: This migration does nothing. It is a left over from code targeting Umbraco 8.
            // It needs to remain, as Umbraco instances that have been upgraded from v8 to v9 will have reached this migration state.
        }
    }
}
