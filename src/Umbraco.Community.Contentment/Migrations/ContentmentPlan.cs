/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Umbraco.Cms.Core.Packaging;

namespace Umbraco.Community.Contentment.Migrations
{
    internal sealed class ContentmentPlan : PackageMigrationPlan
    {
        public override string InitialState => "{contentment-init-state}";

        public ContentmentPlan()
            : base(Constants.Internals.ProjectName)
        { }

        protected override void DefinePlan()
        {
            From(InitialState)
               .To<RegisterUmbracoPackageEntry>(RegisterUmbracoPackageEntry.State)
            ;
        }
    }
}
