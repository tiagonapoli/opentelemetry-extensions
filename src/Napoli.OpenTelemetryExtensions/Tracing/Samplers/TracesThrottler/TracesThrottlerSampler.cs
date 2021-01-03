namespace Napoli.OpenTelemetryExtensions.Tracing.Samplers.TracesThrottler
{
    using System.Diagnostics;
    using Napoli.OpenTelemetryExtensions.Interfaces;
    using OpenTelemetry.Trace;

    public class TracesThrottlerSampler : Sampler, IConfigUpdatableComponent, IMeasurableComponent
    {
        private readonly Sampler _rootSampler;
        private readonly object _lockObj = new object();
        private int _ongoingTraces;
        private int _maxOngoingTraces;

        private int _tracedRequests;
        private int _throttledTraces;
        private readonly IConfigurationProvider _configProvider;

        public TracesThrottlerSampler(Sampler rootSampler, IConfigurationProvider configProvider)
        {
            this.Description = nameof(TracesThrottlerSampler);
            this._rootSampler = rootSampler;
            this._configProvider = configProvider;
        }

        /// <inheritdoc/>
        public void ReportMetrics(IMetricsTracker metricsTracker)
        {
            metricsTracker.Register("TracedRequests", this._tracedRequests);
            metricsTracker.Register("OngoingTraces", this._ongoingTraces);
            metricsTracker.Register("ThrottledTraces", this._throttledTraces);
            this._tracedRequests = 0;
            this._throttledTraces = 0;
        }

        /// <inheritdoc/>
        public void UpdateConfiguration()
        {
            this._maxOngoingTraces = this._configProvider.GetMaxConcurrentTraces();
        }

        /// <inheritdoc/>
        public void ResetConfiguration()
        {
            this._maxOngoingTraces = 0;
        }

        public override SamplingResult ShouldSample(in SamplingParameters samplingParameters)
        {
            if (samplingParameters.Kind != ActivityKind.Server) return this._rootSampler.ShouldSample(samplingParameters);
            var samplingDecision = this._rootSampler.ShouldSample(samplingParameters); ;
            if (samplingDecision.Decision == SamplingDecision.Drop) return samplingDecision;
            lock (this._lockObj)
            {
                if (this._ongoingTraces >= this._maxOngoingTraces)
                {
                    this._throttledTraces++;
                    return new SamplingResult(false);
                }

                this._tracedRequests++;
                this._ongoingTraces++;
            }

            return samplingDecision;
        }

        public void DecrementOngoingTraces()
        {
            lock (this._lockObj)
            {
                this._ongoingTraces--;
            }
        }
    }
}
