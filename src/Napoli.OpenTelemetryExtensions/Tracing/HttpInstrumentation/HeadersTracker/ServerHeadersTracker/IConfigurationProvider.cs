namespace Napoli.OpenTelemetryExtensions.Tracing.HttpInstrumentation.HeadersTracker.ServerHeadersTracker
{
    using System.Collections.Generic;

    public interface IConfigurationProvider
    {
        List<string> GetServerTrackedHeaders();
    }
}
