using Umbraco.Cms.Infrastructure.Migrations;

namespace Umbraco.Community.Contentment.Migrations.Upgrade.V_6_1_3;

// Short lived migration. See https://github.com/leekelleher/umbraco-contentment/pull/539 for details.
internal sealed class MigrateContentBlockLocalLinks : NoopMigration
{
    public const string State = "{contentment-content-block-local-links}";

    public MigrateContentBlockLocalLinks(IMigrationContext context)
        : base(context)
    { }
}
