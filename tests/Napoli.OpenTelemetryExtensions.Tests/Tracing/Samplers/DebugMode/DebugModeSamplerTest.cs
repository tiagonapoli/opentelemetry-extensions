namespace Napoli.OpenTelemetryExtensions.Tests.Tracing.Samplers
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Moq;
    using Napoli.OpenTelemetryExtensions.Tracing.Conventions;
    using Napoli.OpenTelemetryExtensions.Tracing.DelegatingHandlers.StartTraceHandler;
    using Napoli.OpenTelemetryExtensions.Tracing.Samplers.DebugMode;
    using OpenTelemetry.Trace;
    using Xunit;

    public class DebugModeSamplerTest
    {
        private const string DebugModeAttribute = "debug-attribute";
        private static DebugModeSampler GetSampler(Mock<Sampler> mockRootSampler)
        {
            return new(mockRootSampler.Object, new DebugModeConfig("debug-header", DebugModeAttribute));
        }

        private static SamplingParameters GetSamplingParameters(List<KeyValuePair<string, object>> attributes)
        {
            return new(default,
                default,
                "operation-test",
                ActivityKind.Server, attributes);
        }

        [Fact]
        public void RecordIfDebugModeAttributeIsPresent()
        {
            var rootSampler = new Mock<Sampler>();
            var sampler = GetSampler(rootSampler);
            var samplingParameters = GetSamplingParameters(new List<KeyValuePair<string, object>> { new(DebugModeAttribute, "123") });
            var res = sampler.ShouldSample(samplingParameters);
            var samplingAttribute = res.Attributes.First(el => el.Key == OpenTelemetryAttributes.AttributeSamplerType);

            rootSampler.Verify(el => el.ShouldSample(It.IsAny<SamplingParameters>()), Times.Never());
            Assert.Equal(SamplingDecision.RecordAndSample, res.Decision);
            Assert.Equal(samplingAttribute.Value, DebugModeSampler.SamplerType);
        }

        [Fact]
        public void CallRootSamplerIfDebugModeAttributeIsNotPresent()
        {
            var rootSampler = new Mock<Sampler>();
            rootSampler.Setup(el => el.ShouldSample(It.IsAny<SamplingParameters>()))
                .Returns(new SamplingResult(false));
            var sampler = GetSampler(rootSampler);
            var samplingParameters = GetSamplingParameters(new List<KeyValuePair<string, object>>());
            var res = sampler.ShouldSample(samplingParameters);

            rootSampler.Verify(el => el.ShouldSample(It.Ref<SamplingParameters>.IsAny), Times.Once);
            Assert.Equal(SamplingDecision.Drop, res.Decision);
            Assert.Null(res.Attributes);
        }
    }
}
