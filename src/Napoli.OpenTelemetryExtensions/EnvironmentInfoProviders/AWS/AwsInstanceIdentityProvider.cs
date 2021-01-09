namespace Napoli.OpenTelemetryExtensions.EnvironmentInfoProviders.AWS
{
    using System;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Napoli.OpenTelemetryExtensions.Tracing.ResourceEnhancers;

    public static class AwsInstanceIdentityProvider
    {
        public static AwsInstanceIdentity InstanceIdentity;

        public static async Task InitInstanceIdentity(int timeoutMs, Func<string, AwsInstanceIdentity> deserializer,
            CancellationToken cancellationToken, Action<Exception> onInitError = null)
        {
            if (InstanceIdentity != null)
            {
                return;
            }

            using var cancellationTokenSource = new CancellationTokenSource(timeoutMs);
            using var linkedCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, cancellationTokenSource.Token);
            try
            {
                using var httpClient = new HttpClient();
                var client = new AwsMetadataClient(httpClient, deserializer);
                InstanceIdentity = await client.GetInstanceIdentityAsync(linkedCancellationTokenSource.Token);
            }
            catch (Exception ex)
            {
                onInitError?.Invoke(ex);
            }
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
                Region = InstanceIdentity.Region,
                Zone = InstanceIdentity.AvailabilityZone
            };
        }
    }
}
