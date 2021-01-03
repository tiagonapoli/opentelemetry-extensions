namespace Napoli.OpenTelemetryExtensions.Tracing.Samplers.ProbabilisticOrDebugModeSampler
{
    public interface IConfigurationProvider
    {
        public Configuration GetProbabilisticOrDebugModeSamplerConfig();
    }
}
