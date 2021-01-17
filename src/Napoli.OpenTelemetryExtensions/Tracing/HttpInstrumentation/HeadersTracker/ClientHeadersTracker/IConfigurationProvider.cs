namespace Napoli.OpenTelemetryExtensions.Tracing.HttpInstrumentation.HeadersTracker.ClientHeadersTracker
{
    using System.Collections.Generic;

    public interface IConfigurationProvider
    {
        List<string> GetClientTrackedHeaders();
    }
}
