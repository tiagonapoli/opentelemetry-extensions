namespace Napoli.OpenTelemetryExtensions.Tracing.DelegatingHandlers.StartTraceHandler
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Net.Http.Headers;
    using Napoli.OpenTelemetryExtensions.Tracing.Conventions;
    using Napoli.OpenTelemetryExtensions.Utils;

    public class ServerHeadersTracker
    {
        private List<string> _trackedHeaders;

        public ServerHeadersTracker()
        {
            this._trackedHeaders = new List<string>();
        }

        public void EnrichWithRequest(Activity activity, HttpRequestHeaders requestHeaders)
        {
            foreach (var trackedHeader in this._trackedHeaders)
            {
                if (requestHeaders.TryGetHeaderAsString(trackedHeader, out var headerContent))
                {
                    activity.SetTag(OpenTelemetryAttributes.GetAttributeHttpRequestHeader(trackedHeader), headerContent);
                }
            }
        }

        public void EnrichWithResponse(Activity activity, HttpResponseHeaders responseHeaders)
        {
            foreach (var trackedHeader in this._trackedHeaders)
            {
                if (responseHeaders.TryGetHeaderAsString(trackedHeader, out var headerContent))
                {
                    activity.SetTag(OpenTelemetryAttributes.GetAttributeHttpResponseHeader(trackedHeader),
                        headerContent);
                }
            }
        }

        public void UpdateConfiguration(List<string> trackedHeaders)
        {
            this._trackedHeaders = trackedHeaders ?? new List<string>();
        }
    }
}
