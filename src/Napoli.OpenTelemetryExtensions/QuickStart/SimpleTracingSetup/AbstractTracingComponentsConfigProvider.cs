namespace Napoli.OpenTelemetryExtensions.QuickStart.SimpleTracingSetup
{
    using System.Threading;
    using System.Threading.Tasks;
    using Napoli.OpenTelemetryExtensions.Interfaces;
    using ProbabilisticOrDebugModeSamplerConfig = Napoli.OpenTelemetryExtensions.Tracing.Samplers.ProbabilisticOrDebugModeSampler.Configuration;
    using SamplersNamespace = Napoli.OpenTelemetryExtensions.Tracing.Samplers;
    using StartTraceHandlerConfig = Napoli.OpenTelemetryExtensions.Tracing.DelegatingHandlers.StartTraceHandler.Configuration;
    using TracingNamespace = Napoli.OpenTelemetryExtensions.Tracing;

    public abstract class AbstractTracingComponentsConfigProvider :
        SamplersNamespace.TracesThrottler.IConfigurationProvider,
        SamplersNamespace.ProbabilisticOrDebugModeSampler.IConfigurationProvider,
        TracingNamespace.DelegatingHandlers.StartTraceHandler.IConfigurationProvider,
        IUpdatable
    {
        public abstract int GetMaxConcurrentTraces();
        public abstract ProbabilisticOrDebugModeSamplerConfig GetProbabilisticOrDebugModeSamplerConfig();
        public abstract StartTraceHandlerConfig GetStartTraceHandlerConfig();
        public abstract Task UpdateAsync(CancellationToken cancellationToken);
    }
}
