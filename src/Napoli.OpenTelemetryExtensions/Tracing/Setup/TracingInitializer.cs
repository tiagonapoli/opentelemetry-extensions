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
    using Tracer = Napoli.OpenTelemetryExtensions.Tracing.Tracer;

    public static class TracingInitializer
    {
        public static TracerProvider Configure(InstrumentationConfig conf)
        {
            Tracer.InitSingleton(new ActivitySource(conf.ServiceName));
            return Sdk.CreateTracerProviderBuilder()
                .SetResourceBuilder(SetupResourceBuilder(conf))
                .AddAndConfigureHttpClientInstrumentation(conf.HttpInstrumentationEnrichWrapper)
                .AddSource(conf.ServiceName)
                .SetSampler(conf.Sampler)
                .AddOtlpExporter(opt =>
                {
                    opt.Endpoint = conf.LightStepIngestEndpoint;
                    opt.Headers = new Metadata { { "lightstep-access-token", conf.LightStepProjectToken } };
                    opt.Credentials = new SslCredentials();
                })
                .Build();
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
                resourceEnhancer.RegisterResourceAttributes(resourceBuilder);
            }

            return resourceBuilder;
        }
    }
}
