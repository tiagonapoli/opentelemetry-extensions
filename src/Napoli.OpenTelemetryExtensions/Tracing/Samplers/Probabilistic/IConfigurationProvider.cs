namespace Napoli.OpenTelemetryExtensions.Tracing.Samplers.Probabilistic
{
    public interface IConfigurationProvider
    {
        public Configuration GetProbabilisticSamplerConfig();
    }
}
