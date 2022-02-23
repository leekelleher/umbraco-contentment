/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

#if NET472
using Umbraco.Core.Migrations;
#else
using Umbraco.Cms.Core.Packaging;
#endif

namespace Umbraco.Community.Contentment.Migrations
{
#if NET472
    internal sealed class ContentmentPlan : MigrationPlan
#else
    internal sealed class ContentmentPlan : PackageMigrationPlan
#endif
    {
        public override string InitialState => "{contentment-init-state}";

        public ContentmentPlan()
            : base(Constants.Internals.ProjectName)
        {
#if NET472
            From(InitialState)
               .To<RegisterUmbracoPackageEntry>(RegisterUmbracoPackageEntry.State)
            ;
#endif
        }

#if NET472 == false
        protected override void DefinePlan()
        {
            From(InitialState)
               .To<RegisterUmbracoPackageEntry>(RegisterUmbracoPackageEntry.State)
            ;
        }
#endif
    }
}
