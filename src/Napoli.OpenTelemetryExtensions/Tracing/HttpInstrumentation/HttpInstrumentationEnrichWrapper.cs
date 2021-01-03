namespace Napoli.OpenTelemetryExtensions.Tracing.HttpInstrumentation
{
    using System;
    using System.Diagnostics;
    using System.Net;
    using Napoli.OpenTelemetryExtensions.Tracing.Conventions;

    public class HttpInstrumentationEnrichWrapper
    {
        private readonly Action<Activity, HttpWebRequest> _onStart;
        private readonly Action<Activity, HttpWebResponse> _onSuccessEnd;
        private readonly Action<Activity, WebException> _onError;

        public HttpInstrumentationEnrichWrapper(Action<Activity, HttpWebRequest> onStart = null,
            Action<Activity, HttpWebResponse> onSuccessEnd = null, Action<Activity, WebException> onError = null)
        {
            this._onStart = onStart;
            this._onSuccessEnd = onSuccessEnd;
            this._onError = onError;
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

                this._onStart?.Invoke(activity, request);
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

                this._onSuccessEnd?.Invoke(activity, response);
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

                this._onError?.Invoke(activity, ex);
            }
        }
    }
}
