namespace Napoli.OpenTelemetryExtensions.Tracing.Samplers.TracesThrottler
{
    public interface IConfigurationProvider
    {
        public int GetMaxConcurrentTraces();
    }
}
