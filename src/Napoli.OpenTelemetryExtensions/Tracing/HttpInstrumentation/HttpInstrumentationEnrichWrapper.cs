namespace Napoli.OpenTelemetryExtensions.Tracing.HttpInstrumentation
{
    using System.Diagnostics;
    using System.Net;
    using Napoli.OpenTelemetryExtensions.Tracing.Conventions;

    public class HttpInstrumentationEnrichWrapper
    {
        private readonly IHttpEnrichHooks[] _enrichHooks;

        public HttpInstrumentationEnrichWrapper(IHttpEnrichHooks[] enrichHooks)
        {
            this._enrichHooks = enrichHooks ?? new IHttpEnrichHooks[0];
        }

        public void Enrich(Activity activity, string eventName, object rawObject)
        {
            if (eventName.Equals("OnStartActivity"))
            {
                if (!(rawObject is HttpWebRequest request))
                {
                    return;
                }

                if (request.ContentLength > 0)
                {
                    activity.SetTag(OpenTelemetryAttributes.AttributeHttpRequestContentLength,
                        request.ContentLength);
                }

                foreach (var enrichHook in this._enrichHooks)
                {
                    enrichHook.OnStart(activity, request);
                }
            }
            else if (eventName.Equals("OnStopActivity"))
            {
                if (!(rawObject is HttpWebResponse response))
                {
                    return;
                }

                if (response.ContentLength > 0)
                {
                    activity.SetTag(OpenTelemetryAttributes.AttributeHttpResponseContentLength,
                        response.ContentLength);
                }

                foreach (var enrichHook in this._enrichHooks)
                {
                    enrichHook.OnSuccessEnd(activity, response);
                }
            }
            else if (eventName.Equals("OnException"))
            {
                if (!(rawObject is WebException ex))
                {
                    return;
                }

                var statusCode = (ex.Response as HttpWebResponse)?.StatusCode;
                if (statusCode == null)
                {
                    activity.SetTag(OpenTelemetryAttributes.AttributeHttpStatusCode, "-");
                    activity.SetTag(OpenTelemetryAttributes.AttributeHttpClientException, ex.Status.ToString());
                }

                foreach (var enrichHook in this._enrichHooks)
                {
                    enrichHook.OnError(activity, ex);
                }
            }
        }
    }
}
