namespace Napoli.OpenTelemetryExtensions.Tracing.Setup
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using Grpc.Core;
    using Napoli.OpenTelemetryExtensions.Tracing.Conventions;
    using Napoli.OpenTelemetryExtensions.Tracing.HttpInstrumentation;
    using OpenTelemetry;
    using OpenTelemetry.Resources;
    using OpenTelemetry.Trace;

    public static class TracingSetup
    {
        private static TracerProviderBuilder _tracerProviderBuilder;
        private static TracerProvider _tracerProvider;

        /// <summary>
        /// Should be called before any HttpRequest is placed
        /// </summary>
        /// <param name="conf"></param>
        public static void PreConfigure(InstrumentationConfig conf)
        {
            ActivityTracer.InitSingleton(new ActivitySource(conf.ServiceName));
            _tracerProviderBuilder = Sdk.CreateTracerProviderBuilder();
            TracerProviderBuilderHttpConfigExtension
                .AddAndConfigureHttpClientInstrumentation(_tracerProviderBuilder, new HttpInstrumentationEnrichWrapper(conf.HttpInstrumentationEnrichHooks));
        }

        public static TracerProvider Configure(InstrumentationConfig conf)
        {
            conf.CheckCompleteness();
            _tracerProvider = _tracerProviderBuilder
                .SetResourceBuilder(SetupResourceBuilder(conf))
                .AddSource(conf.ServiceName)
                .SetSampler(conf.Sampler)
                .AddOtlpExporter(opt =>
                {
                    opt.Endpoint = conf.LightStepIngestEndpoint;
                    opt.Headers = new Metadata { { "lightstep-access-token", conf.LightStepProjectToken } };
                })
                .Build();

            return _tracerProvider;
        }

        private static ResourceBuilder SetupResourceBuilder(InstrumentationConfig conf)
        {
            var resourceBuilder = ResourceBuilder.CreateEmpty()
                .AddService(conf.ServiceName, serviceVersion: conf.ServiceVersion)
                .AddAttributes(new Dictionary<string, object>
                {
                    {OpenTelemetryResourceAttributes.AttributeDeploymentEnvironment, conf.DeploymentEnvironment},
                    {OpenTelemetryResourceAttributes.AttributeTelemetrySdkLanguage, "dotnet"},
                });

            if (conf.ResourceEnhancers == null)
            {
                return resourceBuilder;
            }

            foreach (var resourceEnhancer in conf.ResourceEnhancers)
            {
                resourceEnhancer?.RegisterResourceAttributes(resourceBuilder);
            }

            return resourceBuilder;
        }
    }
}
