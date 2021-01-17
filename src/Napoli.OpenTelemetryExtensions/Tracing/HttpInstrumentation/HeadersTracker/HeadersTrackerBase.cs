namespace Napoli.OpenTelemetryExtensions.Tracing.HttpInstrumentation.HeadersTracker
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Net;
    using Napoli.OpenTelemetryExtensions.Interfaces;
    using Napoli.OpenTelemetryExtensions.Tracing.Conventions;

    public class HeadersTrackerBase : IConfigUpdatableComponent
    {
        private List<string> _trackedHeaders;
        private readonly Func<List<string>> _getCurrentConfig;

        protected HeadersTrackerBase(Func<List<string>> getCurrentConfig)
        {
            this._getCurrentConfig = getCurrentConfig;
            this.ResetConfiguration();
        }

        public void EnrichWithRequest(Activity activity, WebHeaderCollection requestHeaders)
        {
            foreach (var trackedHeader in this._trackedHeaders)
            {
                var headerContent = requestHeaders.Get(trackedHeader);
                if (headerContent != null)
                {
                    activity.SetTag(OpenTelemetryAttributes.GetAttributeHttpRequestHeader(trackedHeader),
                        headerContent);
                }
            }
        }

        public void EnrichWithResponse(Activity activity, WebHeaderCollection responseHeaders)
        {
            foreach (var trackedHeader in this._trackedHeaders)
            {
                var headerContent = responseHeaders.Get(trackedHeader);
                if (headerContent != null)
                {
                    activity.SetTag(OpenTelemetryAttributes.GetAttributeHttpResponseHeader(trackedHeader),
                        headerContent);
                }
            }
        }

        public void UpdateConfiguration()
        {
            this._trackedHeaders = this._getCurrentConfig();
        }

        public void ResetConfiguration()
        {
            this._trackedHeaders = new List<string>();
        }
    }
}
