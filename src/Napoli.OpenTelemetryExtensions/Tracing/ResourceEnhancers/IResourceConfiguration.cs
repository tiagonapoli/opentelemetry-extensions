namespace Napoli.OpenTelemetryExtensions.Tracing.ResourceEnhancers
{
    using OpenTelemetry.Resources;

    public interface IResourceEnhancer
    {
        public void RegisterResourceAttributes(ResourceBuilder resourceBuilder);
    }
}
