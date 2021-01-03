namespace Napoli.OpenTelemetryExtensions.Tracing.DelegatingHandlers.StartTraceHandler
{
    public class Configuration
    {
        public static Configuration GetDefault()
        {
            return new Configuration(false, false);
        }

        public readonly bool EnableTracing;
        public readonly bool EnableDebugMode;

        public Configuration(bool enableTracing, bool enableDebugMode)
        {
            this.EnableTracing = enableTracing;
            this.EnableDebugMode = enableDebugMode;
        }
    }
}
