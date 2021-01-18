namespace Napoli.OpenTelemetryExtensions.Tracing.DelegatingHandlers.StartTraceHandler.HeadersTracker
{
    using System.Diagnostics;
    using System.Net.Http.Headers;
    using Napoli.OpenTelemetryExtensions.Tracing.Conventions;
    using Napoli.OpenTelemetryExtensions.Utils;

    public class ServerHeadersTracker
    {
        private Configuration _config;

        public ServerHeadersTracker()
        {
            this._config = Configuration.GetDefault();
        }

        public void EnrichWithRequest(Activity activity, HttpRequestHeaders requestHeaders)
        {
            foreach (var trackedHeader in this._config.TrackedRequestHeaders)
            {
                if (requestHeaders.TryGetHeaderAsString(trackedHeader, out var headerContent))
                {
                    activity.SetTag(OpenTelemetryAttributes.GetAttributeHttpRequestHeader(trackedHeader),
                        headerContent);
                }
            }
        }

        public void EnrichWithResponse(Activity activity, HttpResponseHeaders responseHeaders)
        {
            foreach (var trackedHeader in this._config.TrackedResponseHeaders)
            {
                if (responseHeaders.TryGetHeaderAsString(trackedHeader, out var headerContent))
                {
                    activity.SetTag(OpenTelemetryAttributes.GetAttributeHttpResponseHeader(trackedHeader),
                        headerContent);
                }
            }
        }

        public void UpdateConfiguration(Configuration config)
        {
            this._config = config;
        }
    }
}
