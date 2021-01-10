namespace Napoli.OpenTelemetryExtensions.Tests.Tracing.Samplers
{
    using OpenTelemetry.Trace;

    public class MockRootSampler : Sampler
    {
        public bool Sample { get; set; }
        public override SamplingResult ShouldSample(in SamplingParameters samplingParameters)
        {
            return new SamplingResult(this.Sample);
        }
    }
}
