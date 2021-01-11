namespace Napoli.OpenTelemetryExtensions.Tracing.Samplers.Probabilistic
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using Napoli.OpenTelemetryExtensions.Interfaces;
    using Napoli.OpenTelemetryExtensions.Tracing.Conventions;
    using OpenTelemetry.Trace;

    public class ProbabilisticSampler : Sampler, IConfigUpdatableComponent, IMeasurableComponent
    {
        private readonly KeyValuePair<string, object> _probabilisticSamplerType =
            new KeyValuePair<string, object>(OpenTelemetryAttributes.AttributeSamplerType, "probabilistic");

        private Configuration _config;
        private readonly IConfigurationProvider _configProvider;
        private int _probabilisticTracedRequests;

        public ProbabilisticSampler(IConfigurationProvider configProvider)
        {
            this._configProvider = configProvider;
            this.Description = nameof(ProbabilisticSampler);
            this.ResetConfiguration();
        }

        /// <inheritdoc/>
        public void UpdateConfiguration()
        {
            this._config = this._configProvider.GetProbabilisticSamplerConfig();
        }

        /// <inheritdoc/>
        public void ResetConfiguration()
        {
            this._config = Configuration.GetDefault();
        }

        /// <inheritdoc/>
        public void ReportMetrics(IMetricsTracker metricsTracker)
        {
            metricsTracker.Register("ProbabilisticTracedRequests", this._probabilisticTracedRequests);
            this._probabilisticTracedRequests = 0;
        }

        public override SamplingResult ShouldSample(in SamplingParameters samplingParameters)
        {
            Span<byte> traceIdBytes = stackalloc byte[16];
            samplingParameters.TraceId.CopyTo(traceIdBytes);
            var conf = this._config.GetSamplingConfig(samplingParameters.Name);

            if (Math.Abs(GetLowerLong(traceIdBytes)) < conf.Value)
            {
                Interlocked.Increment(ref this._probabilisticTracedRequests);
                return new SamplingResult(SamplingDecision.RecordAndSample,
                    new[]
                    {
                        this._probabilisticSamplerType, new KeyValuePair<string, object>("sampler.param",
                            conf.Key.ToString(CultureInfo.InvariantCulture))
                    });
            }

            return new SamplingResult(SamplingDecision.Drop);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long GetLowerLong(ReadOnlySpan<byte> bytes)
        {
            long result = 0;
            for (var i = 0; i < 8; i++)
            {
                result <<= 8;
#pragma warning disable CS0675 // Bitwise-or operator used on a sign-extended operand
                result |= bytes[i] & 0xff;
#pragma warning restore CS0675 // Bitwise-or operator used on a sign-extended operand
            }

            return result;
        }
    }
}
