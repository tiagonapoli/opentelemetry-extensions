namespace Napoli.OpenTelemetryExtensions.Tracing.DelegatingHandlers.StartTraceHandler
{
    public class DebugModeConfig
    {
        public readonly string DebugModeHeader;
        public readonly string DebugModeAttribute;

        public DebugModeConfig(string debugModeHeader, string debugModeAttribute)
        {
            this.DebugModeHeader = debugModeHeader;
            this.DebugModeAttribute = debugModeAttribute;
        }
    }
}
