namespace Napoli.OpenTelemetryExtensions.Tracing.Conventions
{
    public static class OpenTelemetryResourceAttributes
    {
        // Resource attributes from https://github.com/open-telemetry/opentelemetry-specification/tree/master/specification/resource/semantic_conventions

        public const string AttributeTelemetrySdkLanguage = "telemetry.sdk.language";

        public const string AttributeDeploymentEnvironment = "deployment.environment";

        public const string AttributeHostId = "host.id";
        public const string AttributeHostType = "host.type";
        public const string AttributeHostImageName = "host.image.name";

        public const string AttributeCloudProvider = "cloud.provider";
        public const string AttributeCloudAccountId = "cloud.account.id";
        public const string AttributeCloudRegion = "cloud.region";
        public const string AttributeCloudZone = "cloud.zone";

        public const string AttributeOsType = "os.type";
        public const string AttributeOsDescription = "os.description";
    }
}
