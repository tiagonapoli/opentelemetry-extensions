namespace Napoli.OpenTelemetryExtensions.Tracing.HttpInstrumentation
{
    using System.Diagnostics;
    using System.Net;

    public interface IHttpEnrichHooks
    {
        void OnStart(Activity activity, HttpWebRequest request);
        void OnSuccessEnd(Activity activity, HttpWebResponse response);
        void OnError(Activity activity, WebException ex);
    }
}
