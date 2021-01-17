namespace Napoli.OpenTelemetryExtensions.Tracing.HttpInstrumentation.HeadersTracker.ClientHeadersTracker
{
    using System.Diagnostics;
    using System.Net;
    using System.Runtime.CompilerServices;

    public class ClientHeadersTracker : HeadersTrackerBase, IHttpEnrichHooks
    {
        public ClientHeadersTracker(IConfigurationProvider configurationProvider) : base(configurationProvider.GetClientTrackedHeaders)
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void OnStart(Activity activity, HttpWebRequest request)
        {
            this.EnrichWithRequest(activity, request.Headers);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void OnSuccessEnd(Activity activity, HttpWebResponse response)
        {
            this.EnrichWithResponse(activity, response.Headers);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void OnError(Activity activity, WebException ex)
        {
            var headers = (ex.Response as HttpWebResponse)?.Headers;
            if (headers != null)
            {
                this.EnrichWithResponse(activity, headers);
            }
        }
    }
}
