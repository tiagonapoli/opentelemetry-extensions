namespace Napoli.OpenTelemetryExtensions.Tracing.DelegatingHandlers.StartTraceHandler
{
    using System.Collections.Generic;

    public class Configuration
    {
        public static Configuration GetDefault()
        {
            return new Configuration(false, false, new List<string>());
        }

        public readonly bool EnableTracing;
        public readonly bool EnableDebugMode;
        public readonly List<string> TrackedHeaders;

        public Configuration(bool enableTracing, bool enableDebugMode, List<string> trackedHeaders)
        {
            this.EnableTracing = enableTracing;
            this.EnableDebugMode = enableDebugMode;
            this.TrackedHeaders = trackedHeaders;
        }
    }
}
