namespace Napoli.OpenTelemetryExtensions.Tracing.Setup
{
    using System;
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
        public string LightStepIngestEndpoint;

        public string LightStepProjectToken;

        /**
         * Http instrumentation config
         */
        public readonly List<IHttpEnrichHooks> HttpInstrumentationEnrichHooks;

        public readonly List<IResourceEnhancer> ResourceEnhancers;

        public Sampler Sampler;

        public InstrumentationConfig(string serviceName, string serviceVersion, string deploymentEnvironment)
        {
            this.ServiceName = serviceName;
            this.ServiceVersion = serviceVersion;
            this.DeploymentEnvironment = deploymentEnvironment;
            this.HttpInstrumentationEnrichHooks = new List<IHttpEnrichHooks>();
            this.ResourceEnhancers = new List<IResourceEnhancer>();
        }

        public InstrumentationConfig AddHttpInstrumentationEnrichHooks(IHttpEnrichHooks enrichHooks)
        {
            this.HttpInstrumentationEnrichHooks.Add(enrichHooks);
            return this;
        }

        public InstrumentationConfig WithLightStepConfig(string lightStepIngestEndpoint, string lightStepProjectToken)
        {
            this.LightStepIngestEndpoint = lightStepIngestEndpoint;
            this.LightStepProjectToken = lightStepProjectToken;
            return this;
        }

        public InstrumentationConfig WithSampler(Sampler sampler)
        {
            this.Sampler = sampler;
            return this;
        }

        public InstrumentationConfig AddResourceEnhancers(IEnumerable<IResourceEnhancer> resourceEnhancers)
        {
            foreach (var resourceEnhancer in resourceEnhancers)
            {
                if (resourceEnhancer != null)
                {
                    this.ResourceEnhancers.Add(resourceEnhancer);
                }
            }

            return this;
        }

        public InstrumentationConfig AddResourceEnhancer(IResourceEnhancer resourceEnhancer)
        {
            if (resourceEnhancer != null)
            {
                this.ResourceEnhancers.Add(resourceEnhancer);
            }

            return this;
        }

        public void CheckCompleteness()
        {
            if (this.Sampler == null)
            {
                throw new ArgumentNullException(nameof(this.Sampler));
            }

            if (this.LightStepIngestEndpoint == null)
            {
                throw new ArgumentNullException(nameof(this.LightStepIngestEndpoint));
            }

            if (this.LightStepProjectToken == null)
            {
                throw new ArgumentNullException(nameof(this.LightStepProjectToken));
            }
        }
    }
}
