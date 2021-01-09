namespace Napoli.OpenTelemetryExtensions.Tracing.HttpInstrumentation
{
    using System.Diagnostics;
    using System.Net;
    using Napoli.OpenTelemetryExtensions.Tracing.Conventions;

    public class HttpInstrumentationEnrichWrapper
    {
        private readonly IHttpEnrichHooks _enrichHooks;

        public HttpInstrumentationEnrichWrapper(IHttpEnrichHooks enrichHooks = null)
        {
            this._enrichHooks = enrichHooks ?? new DefaultHttpEnrichHooks();
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

                this._enrichHooks.OnStart(activity, request);
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

                this._enrichHooks.OnSuccessEnd(activity, response);
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
                }

                this._enrichHooks.OnError(activity, ex);
            }
        }
    }
}
