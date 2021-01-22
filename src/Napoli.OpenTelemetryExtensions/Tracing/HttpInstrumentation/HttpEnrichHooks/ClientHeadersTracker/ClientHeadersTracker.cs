namespace Napoli.OpenTelemetryExtensions.Tracing.HttpInstrumentation.HttpEnrichHooks.ClientHeadersTracker
{
    using System.Diagnostics;
    using System.Net;
    using System.Runtime.CompilerServices;
    using Napoli.OpenTelemetryExtensions.Interfaces;
    using Napoli.OpenTelemetryExtensions.Tracing.Conventions;

    public class ClientHeadersTracker : IConfigUpdatableComponent, IHttpEnrichHooks
    {
        private readonly IConfigurationProvider _configurationProvider;
        private Configuration _config;

        public ClientHeadersTracker(IConfigurationProvider configurationProvider)
        {
            this._configurationProvider = configurationProvider;
        }

        public void UpdateConfiguration()
        {
            this._config = this._configurationProvider.GetClientTrackedHeadersConfig();
        }

        public void ResetConfiguration()
        {
            this._config = Configuration.GetDefault();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void OnStart(Activity activity, HttpWebRequest request)
        {
            foreach (var trackedHeader in this._config.TrackedRequestHeaders)
            {
                var headerContent = request.Headers.Get(trackedHeader);
                if (headerContent != null)
                {
                    activity.SetTag(OpenTelemetryAttributes.GetAttributeHttpRequestHeader(trackedHeader),
                        headerContent);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void OnSuccessEnd(Activity activity, HttpWebResponse response)
        {
            this.EnrichWithResponseHeaders(activity, response.Headers);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void OnError(Activity activity, WebException ex)
        {
            var headers = (ex.Response as HttpWebResponse)?.Headers;
            if (headers != null)
            {
                this.EnrichWithResponseHeaders(activity, headers);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void EnrichWithResponseHeaders(Activity activity, WebHeaderCollection headers)
        {
            foreach (var trackedHeader in this._config.TrackedResponseHeaders)
            {
                var headerContent = headers.Get(trackedHeader);
                if (headerContent != null)
                {
                    activity.SetTag(OpenTelemetryAttributes.GetAttributeHttpResponseHeader(trackedHeader),
                        headerContent);
                }
            }
        }
    }
}
