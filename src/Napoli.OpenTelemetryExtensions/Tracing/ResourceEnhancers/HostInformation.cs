namespace Napoli.OpenTelemetryExtensions.Tracing.ResourceEnhancers
{
    using System.Collections.Generic;
    using Napoli.OpenTelemetryExtensions.Tracing.Conventions;
    using OpenTelemetry;
    using OpenTelemetry.Resources;

    public class HostInformation : IResourceEnhancer
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public string ImageId { get; set; }

        public void RegisterResourceAttributes(ResourceBuilder resourceBuilder)
        {
            var attributes = new Dictionary<string, object>();

            if (this.Id != null)
            {
                attributes.Add(OpenTelemetryResourceAttributes.AttributeHostId, this.Id);
            }

            if (this.Type != null)
            {
                attributes.Add(OpenTelemetryResourceAttributes.AttributeHostType, this.Type);
            }

            if (this.ImageId != null)
            {
                attributes.Add(OpenTelemetryResourceAttributes.AttributeHostImageName, this.ImageId);
            }

            resourceBuilder.AddAttributes(attributes);
        }
    }
}
