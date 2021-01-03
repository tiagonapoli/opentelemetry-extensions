namespace Napoli.OpenTelemetryExtensions.EnvironmentInfoProviders.AWS
{
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Napoli.OpenTelemetryExtensions.Tracing.ResourceEnhancers;

    public class AwsInstanceIdentityProvider
    {
        public static AwsInstanceIdentity InstanceIdentity;

        public static async Task<AwsInstanceIdentity> InitInstanceIdentity(int timeout,
            CancellationToken cancellationToken)
        {
            if (InstanceIdentity != null)
            {
                return InstanceIdentity;
            }

            using var cancellationTokenSource = new CancellationTokenSource(timeout);
            using var linkedCancellationTokenSource =
                CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, cancellationTokenSource.Token);
            using var httpClient = new HttpClient();
            var client = new AwsMetadataClient(httpClient);
            InstanceIdentity = await client.GetInstanceIdentityAsync(linkedCancellationTokenSource.Token);

            return InstanceIdentity;
        }

        public static IResourceEnhancer GetHostResourceEnhancer()
        {
            if (InstanceIdentity == null)
            {
                return null;
            }

            return new HostInformation
            {
                Id = InstanceIdentity.InstanceId,
                Type = InstanceIdentity.InstanceType,
                ImageId = InstanceIdentity.ImageId,
            };
        }

        public static IResourceEnhancer GetCloudResourceEnhancer()
        {
            if (InstanceIdentity == null)
            {
                return null;
            }

            return new CloudInformation
            {
                Provider = "aws",
                AccountId = InstanceIdentity.AccountId,
                Region = InstanceIdentity.Region,
                Zone = InstanceIdentity.AvailabilityZone
            };
        }
    }
}
