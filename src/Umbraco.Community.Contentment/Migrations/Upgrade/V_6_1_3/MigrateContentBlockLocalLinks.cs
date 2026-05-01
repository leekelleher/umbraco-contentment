using Umbraco.Cms.Infrastructure.Migrations;

namespace Umbraco.Community.Contentment.Migrations.Upgrade.V_6_1_3;

[Obsolete("To be removed in Contentment 7.0, as `ITypedLocalLinkProcessor` will be removed in Umbraco 18.0.")]
internal sealed class MigrateContentBlockLocalLinks : NoopMigration
{
    public const string State = "{contentment-content-block-local-links}";

    public MigrateContentBlockLocalLinks(IMigrationContext context)
        : base(context)
    { }
}
