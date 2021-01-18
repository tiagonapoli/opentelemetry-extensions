namespace Napoli.OpenTelemetryExtensions.Tracing.DelegatingHandlers.StartTraceHandler
{
    using System.Collections.Generic;

    public class Configuration
    {
        public static Configuration GetDefault()
        {
            return new Configuration(false, false, HeadersTracker.Configuration.GetDefault());
        }

        public readonly bool EnableTracing;
        public readonly bool EnableDebugMode;
        public readonly HeadersTracker.Configuration HeadersTrackingConfig;

        public Configuration(bool enableTracing, bool enableDebugMode, HeadersTracker.Configuration headersTrackingConfig)
        {
            this.EnableTracing = enableTracing;
            this.EnableDebugMode = enableDebugMode;
            this.HeadersTrackingConfig = headersTrackingConfig;
        }
    }
}
