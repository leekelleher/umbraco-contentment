/* Copyright © 2019 Lee Kelleher, Umbrella Inc and other contributors.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Umbraco.Community.Contentment.Migrations.Install;
using Umbraco.Core.Migrations;

namespace Umbraco.Community.Contentment.Migrations
{
    internal class ContentmentPlan : MigrationPlan
    {
        public ContentmentPlan()
            : base(Constants.Internals.ProjectName)
        {
            From(string.Empty)
                .To<RegisterUmbracoPackageEntry>("{92FFD63F-44DC-4555-A347-87C5B1D24D6E}")
            ;
        }
    }
}
