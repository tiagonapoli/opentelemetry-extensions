namespace Napoli.OpenTelemetryExtensions.EnvironmentInfoProviders.AWS
{
    using System;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;

    public class AwsMetadataClient
    {
        private readonly HttpClient _client;
        private readonly Func<string, AwsInstanceIdentity> _deserializer;

        public AwsMetadataClient(HttpClient client, Func<string, AwsInstanceIdentity> deserializer)
        {
            this._client = client;
            this._deserializer = deserializer;
        }

        public async Task<AwsInstanceIdentity> GetInstanceIdentityAsync(CancellationToken cancellationToken)
        {
            var uri = new Uri("http://169.254.169.254/latest/dynamic/instance-identity/document");
            using var req = new HttpRequestMessage(HttpMethod.Get, uri);
            using var res = await this._client.SendAsync(req, cancellationToken);
            var content = await res.Content.ReadAsStringAsync().ConfigureAwait(false);
            if (res.IsSuccessStatusCode)
            {
                return content == null ? null : this._deserializer(content);
            }

            res.Content?.Dispose();
            return null;
        }
    }
}
