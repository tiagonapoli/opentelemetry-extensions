namespace Napoli.OpenTelemetryExtensions.Tracing.ResourceEnhancers
{
    using System.Collections.Generic;
    using Napoli.OpenTelemetryExtensions.Tracing.Conventions;
    using OpenTelemetry;
    using OpenTelemetry.Resources;

    public class OsInformation : IResourceEnhancer
    {
        public string Type { get; set; }
        public string Description { get; set; }


        public void RegisterResourceAttributes(ResourceBuilder resourceBuilder)
        {
            var attributes = new Dictionary<string, object>();

            if (this.Type != null)
            {
                attributes.Add(OpenTelemetryResourceAttributes.AttributeOsType, this.Type);
            }

            if (this.Description != null)
            {
                attributes.Add(OpenTelemetryResourceAttributes.AttributeOsDescription, this.Description);
            }

            resourceBuilder.AddAttributes(attributes);
        }
    }
}
