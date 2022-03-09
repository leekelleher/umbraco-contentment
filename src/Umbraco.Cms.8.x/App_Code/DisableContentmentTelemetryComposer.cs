using Umbraco.Core.Composing;

namespace Umbraco.Cms._8.x
{
    public class DisableContentmentTelemetryComposer : IUserComposer
    {
        public void Compose(Composition composition)
        {
            composition.DisableContentmentTelemetry();
        }
    }
}
