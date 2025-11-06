/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Umbraco.Cms.Core.Packaging;
using Umbraco.Cms.Infrastructure.Migrations;

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
               .To<Upgrade.V_6_0_0.AddEditorUiToDataType>(Upgrade.V_6_0_0.AddEditorUiToDataType.State)
               .To<Upgrade.V_6_0_0.MigrateNotesConfiguration>(Upgrade.V_6_0_0.MigrateNotesConfiguration.State)
               .To<Upgrade.V_6_0_0.MigrateEditorNotesConfiguration>(Upgrade.V_6_0_0.MigrateEditorNotesConfiguration.State)
               .To<Upgrade.V_6_0_0.MigrateDataListConfiguration>(Upgrade.V_6_0_0.MigrateDataListConfiguration.State)
               // TODO: [LK] Added no-op to rerun the Data Picker migration. Fix up after beta001.
               .To<NoopMigration>(Upgrade.V_6_0_0.MigrateDataPickerConfiguration.State)
               .To<Upgrade.V_6_0_0.MigrateDataPickerConfiguration>("beta001:" + Upgrade.V_6_0_0.MigrateDataPickerConfiguration.State)
               .To<Upgrade.V_6_0_0.MigrateContentBlocksConfiguration>(Upgrade.V_6_0_0.MigrateContentBlocksConfiguration.State)
               .To<Upgrade.V_6_0_0.MigrateCodeEditorSchema>(Upgrade.V_6_0_0.MigrateCodeEditorSchema.State)
            ;
        }
    }
}
