namespace Napoli.OpenTelemetryExtensions.Tracing.DelegatingHandlers.StartTraceHandler.HeadersTracker
{
    using System.Collections.Generic;

    public class Configuration
    {
        public static Configuration GetDefault()
        {
            return new Configuration();
        }

        public readonly List<string> TrackedResponseHeaders;
        public readonly List<string> TrackedRequestHeaders;

        public Configuration(List<string> trackedRequestHeaders = null, List<string> trackedResponseHeaders = null)
        {
            this.TrackedResponseHeaders = trackedResponseHeaders ?? new List<string>();
            this.TrackedRequestHeaders = trackedRequestHeaders ?? new List<string>();
        }
    }
}
