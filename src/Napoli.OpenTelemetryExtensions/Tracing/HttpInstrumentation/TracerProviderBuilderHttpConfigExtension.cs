namespace Napoli.OpenTelemetryExtensions.Tracing.HttpInstrumentation
{
    using System.Diagnostics;
    using OpenTelemetry.Trace;

    public static class TracerProviderBuilderHttpConfigExtension
    {
        public static TracerProviderBuilder AddAndConfigureHttpClientInstrumentation(this TracerProviderBuilder builder,
            HttpInstrumentationEnrichWrapper enrichWrapper)
        {
#if NETFRAMEWORK
                return builder.AddHttpClientInstrumentation(
                    opt =>
                    {
                        opt.Filter = _ => Activity.Current?.IsAllDataRequested ?? false;
                        opt.Enrich = Enrich;
                    },
                    opt =>
                    {
                        opt.Filter = _ => Activity.Current?.IsAllDataRequested ?? false;
                        opt.Enrich = Enrich;
                    }
                );
#else
            return builder.AddHttpClientInstrumentation(
                opt =>
                {
                    opt.Filter = _ => Activity.Current?.IsAllDataRequested ?? false;
                    opt.Enrich = enrichWrapper.Enrich;
                }
            );
#endif
        }
    }
}
