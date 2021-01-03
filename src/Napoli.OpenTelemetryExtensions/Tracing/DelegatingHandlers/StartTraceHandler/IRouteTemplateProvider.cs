namespace Napoli.OpenTelemetryExtensions.Tracing.DelegatingHandlers.StartTraceHandler
{
    using System.Net.Http;

    public interface IRouteTemplateProvider
    {
        public string GetRouteTemplate(HttpRequestMessage request);
    }
}
