namespace Napoli.OpenTelemetryExtensions.Tracing.DelegatingHandlers.StartTraceHandler
{
    public interface IConfigurationProvider
    {
        Configuration GetStartTraceHandlerConfig();
    }
}
