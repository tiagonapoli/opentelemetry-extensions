namespace Napoli.OpenTelemetryExtensions.Tracing.HttpInstrumentation
{
    using System.Diagnostics;
    using System.Net;

    public class DefaultHttpEnrichHooks : IHttpEnrichHooks
    {
        public void OnStart(Activity activity, HttpWebRequest request)
        {
        }

        public void OnSuccessEnd(Activity activity, HttpWebResponse response)
        {
        }

        public void OnError(Activity activity, WebException ex)
        {
        }
    }
}
