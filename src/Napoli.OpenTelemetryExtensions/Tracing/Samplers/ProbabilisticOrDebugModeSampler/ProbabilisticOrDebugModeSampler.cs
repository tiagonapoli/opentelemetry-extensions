namespace Napoli.OpenTelemetryExtensions.Tracing.Samplers.ProbabilisticOrDebugModeSampler
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using Napoli.OpenTelemetryExtensions.Interfaces;
    using Napoli.OpenTelemetryExtensions.Tracing.Conventions;
    using Napoli.OpenTelemetryExtensions.Tracing.DelegatingHandlers.StartTraceHandler;
    using OpenTelemetry.Trace;

    public class ProbabilisticOrDebugModeSampler : Sampler, IConfigUpdatableComponent
    {
        public const string DebugModeSamplerType = "debug_mode";
        public const string ProbabilisticSamplerType = "probabilistic";
        private readonly KeyValuePair<string, object> _probabilisticSamplerType = new KeyValuePair<string, object>(OpenTelemetryAttributes.AttributeSamplerType, "probabilistic");
        private readonly KeyValuePair<string, object> _debugModeSamplerType = new KeyValuePair<string, object>(OpenTelemetryAttributes.AttributeSamplerType, "debug_mode");
        private Configuration _config;
        private readonly string _debugModeAttribute;
        private readonly IConfigurationProvider _configProvider;

        public ProbabilisticOrDebugModeSampler(DebugModeConfig debugModeConfig, IConfigurationProvider configProvider)
        {
            this._debugModeAttribute = debugModeConfig.DebugModeAttribute;
            this._configProvider = configProvider;
            this.Description = nameof(ProbabilisticOrDebugModeSampler);

            this.ResetConfiguration();
        }

        /// <inheritdoc/>
        public void UpdateConfiguration()
        {
            this._config = this._configProvider.GetProbabilisticOrDebugModeSamplerConfig();
        }

        /// <inheritdoc/>
        public void ResetConfiguration()
        {
            this._config = Configuration.GetDefault();
        }

        public override SamplingResult ShouldSample(in SamplingParameters samplingParameters)
        {
            if (samplingParameters.Tags?.FirstOrDefault(el => el.Key == this._debugModeAttribute).Value !=
                null)
            {
                return new SamplingResult(SamplingDecision.RecordAndSample, new[] { this._debugModeSamplerType });
            }

            Span<byte> traceIdBytes = stackalloc byte[16];
            samplingParameters.TraceId.CopyTo(traceIdBytes);
            var conf = this._config.GetSamplingConfig(samplingParameters.Name);

            if (Math.Abs(GetLowerLong(traceIdBytes)) < conf.Value)
            {
                return new SamplingResult(SamplingDecision.RecordAndSample, new[]
                {
                    this._probabilisticSamplerType,
                    new KeyValuePair<string, object>("sampler.param", conf.Key.ToString(CultureInfo.InvariantCulture))
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
