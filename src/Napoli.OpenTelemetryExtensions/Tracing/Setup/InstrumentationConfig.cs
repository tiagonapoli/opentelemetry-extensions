namespace Napoli.OpenTelemetryExtensions.Tracing.Setup
{
    using System.Collections.Generic;
    using Napoli.OpenTelemetryExtensions.Tracing.HttpInstrumentation;
    using Napoli.OpenTelemetryExtensions.Tracing.ResourceEnhancers;
    using OpenTelemetry.Trace;

    public class InstrumentationConfig
    {
        public readonly string ServiceName;
        public readonly string ServiceVersion;

        /**
         * Environment information
         */
        public readonly string DeploymentEnvironment;

        /**
        * LightStep exporter config
        */
        public readonly string LightStepIngestEndpoint;

        public readonly string LightStepProjectToken;

        /**
         * Http instrumentation config
         */
        public HttpInstrumentationEnrichWrapper HttpInstrumentationEnrichWrapper { get; private set; }

        public readonly List<IResourceEnhancer> ResourceEnhancers;
        public readonly Sampler Sampler;

        public InstrumentationConfig(string serviceName, string serviceVersion, string deploymentEnvironment,
            string lightStepIngestEndpoint, string lightStepProjectToken, Sampler sampler)
        {
            this.ServiceName = serviceName;
            this.ServiceVersion = serviceVersion;
            this.DeploymentEnvironment = deploymentEnvironment;
            this.Sampler = sampler;
            this.LightStepIngestEndpoint = lightStepIngestEndpoint;
            this.LightStepProjectToken = lightStepProjectToken;
            this.HttpInstrumentationEnrichWrapper = new HttpInstrumentationEnrichWrapper();
            this.ResourceEnhancers = new List<IResourceEnhancer>();
        }

        public InstrumentationConfig SetHttpInstrumentationEnrichWrapper(HttpInstrumentationEnrichWrapper newVal)
        {
            this.HttpInstrumentationEnrichWrapper = newVal;
            return this;
        }
    }
}
