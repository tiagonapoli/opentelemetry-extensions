namespace Napoli.OpenTelemetryExtensions.Tracing.HttpInstrumentation.HttpEnrichHooks.ClientHeadersTracker
{
    public interface IConfigurationProvider
    {
         Configuration GetClientTrackedHeadersConfig();
    }
}
