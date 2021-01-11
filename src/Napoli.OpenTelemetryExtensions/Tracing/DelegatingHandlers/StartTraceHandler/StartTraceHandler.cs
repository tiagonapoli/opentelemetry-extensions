namespace Napoli.OpenTelemetryExtensions.Tracing.DelegatingHandlers.StartTraceHandler
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Net.Http;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;
    using Napoli.OpenTelemetryExtensions.Interfaces;
    using Napoli.OpenTelemetryExtensions.Tracing.Conventions;
    using OpenTelemetry;
    using OpenTelemetry.Context.Propagation;
    using OpenTelemetry.Trace;

    public class StartTraceHandler : DelegatingHandler, IConfigUpdatableComponent, IMeasurableComponent
    {
        private static readonly Func<HttpRequestMessage, string, IEnumerable<string>> HttpRequestHeaderValuesGetter =
            (request, name) => request.Headers.TryGetValues(name, out var ret) ? ret : null;

        private Configuration _conf;
        private readonly IConfigurationProvider _configurationProvider;
        private readonly Action _onTraceEnd;
        private readonly DebugModeConfig _debugModeConfig;
        private readonly IRouteTemplateProvider _routeTemplateProvider;
        private readonly ActivityTracer _activityTracer;

        private int _incomingTracedRequests;
        private int _tracedRequests;

        public StartTraceHandler(ActivityTracer activityTracer, Action onTraceEnd, IRouteTemplateProvider routeTemplateProvider,
            DebugModeConfig debugModeConfig, IConfigurationProvider configurationProvider)
        {
            this._activityTracer = activityTracer;
            this._onTraceEnd = onTraceEnd;
            this._routeTemplateProvider = routeTemplateProvider;
            this._debugModeConfig = debugModeConfig;
            this._configurationProvider = configurationProvider;
            this.ResetConfiguration();
        }

        /// <inheritdoc/>
        public void UpdateConfiguration()
        {
            this._conf = this._configurationProvider.GetStartTraceHandlerConfig();
        }

        /// <inheritdoc/>
        public void ResetConfiguration()
        {
            this._conf = Configuration.GetDefault();
        }

        /// <inheritdoc/>
        public void ReportMetrics(IMetricsTracker metricsTracker)
        {
            metricsTracker.Register("TotalTracedRequests", this._tracedRequests);
            metricsTracker.Register("IncomingTracedRequests", this._incomingTracedRequests);
            this._tracedRequests = 0;
            this._incomingTracedRequests = 0;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            if (this._conf.EnableTracing == false)
            {
                if (Activity.Current != null) Activity.Current.IsAllDataRequested = false;
                return await base.SendAsync(request, cancellationToken);
            }

            var headers = request.Headers.ToDictionary(item => item.Key.ToLower(), item => item.Value.First());
            ActivityTagsCollection activityTags = null;

            if (this._conf.EnableDebugMode &&
                headers.TryGetValue(this._debugModeConfig.DebugModeHeader, out var debugModeString))
            {
                activityTags = new ActivityTagsCollection { { this._debugModeConfig.DebugModeAttribute, debugModeString } };
            }

            var routeTemplate = this._routeTemplateProvider.GetRouteTemplate(request);
            var textMapPropagator = Propagators.DefaultTextMapPropagator;
            var ctx = textMapPropagator.Extract(default, request, HttpRequestHeaderValuesGetter);

            using var activity = this._activityTracer.StartEntrypointActivity(
                string.IsNullOrWhiteSpace(routeTemplate) ? $"{request.Method} unknown" : $"{request.Method} {routeTemplate}",
                ActivityKind.Server, ctx, activityTags);

            if (Baggage.Current != default)
            {
                Baggage.Current = ctx.Baggage;
            }

            var isTraced = activity.IsAllDataRequested;
            if (isTraced)
            {
                Interlocked.Increment(ref this._tracedRequests);
                if ((ctx.ActivityContext.TraceFlags & ActivityTraceFlags.Recorded) != 0)
                {
                    Interlocked.Increment(ref this._incomingTracedRequests);
                }
            }

            HttpResponseMessage response = null;

            try
            {
                response = await base.SendAsync(request, cancellationToken);
                return response;
            }
            catch (Exception ex)
            {
                if (activity.IsAllDataRequested)
                {
                    activity.SetTag(OpenTelemetryAttributes.AttributeExceptionType,
                        ex.GetType().ToString());
                }

                throw;
            }
            finally
            {
                if (isTraced)
                {
                    this._onTraceEnd();
                }

                if (activity.IsAllDataRequested)
                {
                    EnrichActivity(request, response, activity);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void EnrichActivity(HttpRequestMessage request, HttpResponseMessage response, Activity activity)
        {
            if (request.RequestUri.Port == 80 || request.RequestUri.Port == 443)
            {
                activity.SetTag(OpenTelemetryAttributes.AttributeHttpHost, request.RequestUri.Host);
            }
            else
            {
                activity.SetTag(OpenTelemetryAttributes.AttributeHttpHost,
                    request.RequestUri.Host + ":" + request.RequestUri.Port);
            }

            activity.SetTag(OpenTelemetryAttributes.AttributeHttpMethod, request.Method);
            activity.SetTag(OpenTelemetryAttributes.AttributeHttpUserAgent, request.Headers.UserAgent);
            activity.SetTag(OpenTelemetryAttributes.AttributeHttpUrl, request.RequestUri.ToString());

            if (response == null) return;
            activity.SetTag(OpenTelemetryAttributes.AttributeHttpStatusCode, (int)response.StatusCode);
            if (activity.GetStatus().StatusCode == StatusCode.Unset)
            {
                activity.SetStatus(TracingUtils.ResolveSpanStatusForHttpStatusCode((int)response.StatusCode));
            }
        }
    }
}
