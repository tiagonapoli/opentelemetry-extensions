namespace Napoli.OpenTelemetryExtensions.Tracing.ResourceEnhancers
{
    using System.Collections.Generic;
    using Napoli.OpenTelemetryExtensions.Tracing.Conventions;
    using OpenTelemetry;
    using OpenTelemetry.Resources;

    public class CloudInformation : IResourceEnhancer
    {
        public string Provider { get; set; }
        public string Region { get; set; }
        public string Zone { get; set; }

        public void RegisterResourceAttributes(ResourceBuilder resourceBuilder)
        {
            var attributes = new Dictionary<string, object>();

            if (this.Provider != null)
            {
                attributes.Add(OpenTelemetryResourceAttributes.AttributeCloudProvider, this.Provider);
            }

            if (this.Region != null)
            {
                attributes.Add(OpenTelemetryResourceAttributes.AttributeCloudRegion, this.Region);
            }

            if (this.Zone != null)
            {
                attributes.Add(OpenTelemetryResourceAttributes.AttributeCloudZone, this.Zone);
            }

            resourceBuilder.AddAttributes(attributes);
        }
    }
}
