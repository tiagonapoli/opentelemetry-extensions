namespace Napoli.OpenTelemetryExtensions.EnvironmentInfoProviders.AWS
{
    public class AwsInstanceIdentity
    {
        public string PrivateIp { get; set; }
        public string AvailabilityZone { get; set; }
        public string Version { get; set; }
        public string InstanceId { get; set; }
        public string InstanceType { get; set; }
        public string ImageId { get; set; }
        public string AccountId { get; set; }
        public string Architecture { get; set; }
        public string Region { get; set; }
    }
}
