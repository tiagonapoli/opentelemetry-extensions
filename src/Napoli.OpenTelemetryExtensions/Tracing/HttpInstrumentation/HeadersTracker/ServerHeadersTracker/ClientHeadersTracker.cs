namespace Napoli.OpenTelemetryExtensions.Tracing.HttpInstrumentation.HeadersTracker.ServerHeadersTracker
{
    public class ServerHeadersTracker : HeadersTrackerBase
    {
        protected ServerHeadersTracker(IConfigurationProvider configurationProvider) : base(configurationProvider.GetServerTrackedHeaders)
        {
        }
    }
}
