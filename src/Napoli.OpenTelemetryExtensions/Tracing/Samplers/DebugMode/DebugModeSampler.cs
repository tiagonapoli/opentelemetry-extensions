namespace Napoli.OpenTelemetryExtensions.Tracing.Samplers.DebugMode
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using Napoli.OpenTelemetryExtensions.Interfaces;
    using Napoli.OpenTelemetryExtensions.Tracing.Conventions;
    using Napoli.OpenTelemetryExtensions.Tracing.DelegatingHandlers.StartTraceHandler;
    using OpenTelemetry.Trace;

    public class DebugModeSampler : Sampler, IMeasurableComponent
    {
        public const string SamplerType = "debug_mode";
        private readonly Sampler _rootSampler;
        private readonly KeyValuePair<string, object> _samplerAttributes = new KeyValuePair<string, object>(OpenTelemetryAttributes.AttributeSamplerType, SamplerType);
        private readonly string _debugModeAttribute;
        private int _debugModeTracedRequests;

        public DebugModeSampler(Sampler rootSampler, DebugModeConfig debugModeConfig)
        {
            this._rootSampler = rootSampler;
            this._debugModeAttribute = debugModeConfig.DebugModeAttribute;
            this.Description = nameof(DebugModeSampler);
        }

        /// <inheritdoc/>
        public void ReportMetrics(IMetricsTracker metricsTracker)
        {
            metricsTracker.Register("DebugModeTracedRequests", this._debugModeTracedRequests);
            this._debugModeTracedRequests = 0;
        }

        public override SamplingResult ShouldSample(in SamplingParameters samplingParameters)
        {
            if (samplingParameters.Tags?.FirstOrDefault(el => el.Key == this._debugModeAttribute).Value != null)
            {
                Interlocked.Increment(ref this._debugModeTracedRequests);
                return new SamplingResult(SamplingDecision.RecordAndSample, new[] { this._samplerAttributes });
            }

            return this._rootSampler.ShouldSample(samplingParameters);
        }
    }
}
