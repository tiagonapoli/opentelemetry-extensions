namespace Napoli.OpenTelemetryExtensions.QuickStart.SimpleTracingSetup
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Napoli.OpenTelemetryExtensions.Interfaces;
    using ClientHeadersTrackerNamespace = Napoli.OpenTelemetryExtensions.Tracing.HttpInstrumentation.HeadersTracker.ClientHeadersTracker;
    using ProbabilisticSamplerConfig = Napoli.OpenTelemetryExtensions.Tracing.Samplers.Probabilistic.Configuration;
    using SamplersNamespace = Napoli.OpenTelemetryExtensions.Tracing.Samplers;
    using StartTraceHandlerConfig = Napoli.OpenTelemetryExtensions.Tracing.DelegatingHandlers.StartTraceHandler.Configuration;
    using TracingNamespace = Napoli.OpenTelemetryExtensions.Tracing;

    public abstract class AbstractTracingComponentsConfigProvider :
        SamplersNamespace.TracesThrottler.IConfigurationProvider,
        SamplersNamespace.Probabilistic.IConfigurationProvider,
        TracingNamespace.DelegatingHandlers.StartTraceHandler.IConfigurationProvider,
        ClientHeadersTrackerNamespace.IConfigurationProvider,
        IUpdatable
    {
        public abstract int GetMaxConcurrentTraces();
        public abstract ProbabilisticSamplerConfig GetProbabilisticSamplerConfig();
        public abstract StartTraceHandlerConfig GetStartTraceHandlerConfig();
        public abstract Task UpdateAsync(CancellationToken cancellationToken);
        public abstract List<string> GetClientTrackedHeaders();
    }
}
