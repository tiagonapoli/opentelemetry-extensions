namespace Napoli.OpenTelemetryExtensions.Tracing.DelegatingHandlers.StartTraceHandler
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Net.Http;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;
    using Napoli.OpenTelemetryExtensions.Interfaces;
    using Napoli.OpenTelemetryExtensions.Tracing.Conventions;
    using Napoli.OpenTelemetryExtensions.Tracing.DelegatingHandlers.StartTraceHandler.HeadersTracker;
    using Napoli.OpenTelemetryExtensions.Utils;
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
        private readonly ServerHeadersTracker _serverHeadersTracker;

        private int _incomingTracedRequests;
        private int _incomingNotTracedRequests;
        private int _incomingUndecidedRequests;
        private int _tracedRequests;

        public StartTraceHandler(ActivityTracer activityTracer, Action onTraceEnd, IRouteTemplateProvider routeTemplateProvider,
            DebugModeConfig debugModeConfig, IConfigurationProvider configurationProvider)
        {
            this._activityTracer = activityTracer;
            this._onTraceEnd = onTraceEnd;
            this._routeTemplateProvider = routeTemplateProvider;
            this._debugModeConfig = debugModeConfig;
            this._configurationProvider = configurationProvider;
            this._serverHeadersTracker = new ServerHeadersTracker();
            this.ResetConfiguration();
        }

        /// <inheritdoc/>
        public void UpdateConfiguration()
        {
            this._conf = this._configurationProvider.GetStartTraceHandlerConfig();
            this._serverHeadersTracker.UpdateConfiguration(this._conf.HeadersTrackingConfig);
        }

        /// <inheritdoc/>
        public void ResetConfiguration()
        {
            this._conf = Configuration.GetDefault();
            this._serverHeadersTracker.UpdateConfiguration(this._conf.HeadersTrackingConfig);
        }

        /// <inheritdoc/>
        public void ReportMetrics(IMetricsTracker metricsTracker)
        {
            metricsTracker.Register("TotalTracedRequests", this._tracedRequests);
            metricsTracker.Register("IncomingTracedRequests", this._incomingTracedRequests);
            metricsTracker.Register("IncomingNotTracedRequests", this._incomingNotTracedRequests);
            metricsTracker.Register("IncomingUndecidedRequests", this._incomingUndecidedRequests);
            this._tracedRequests = 0;
            this._incomingTracedRequests = 0;
            this._incomingNotTracedRequests = 0;
            this._incomingUndecidedRequests = 0;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            if (this._conf.EnableTracing == false)
            {
                if (Activity.Current != null) Activity.Current.IsAllDataRequested = false;
                return await base.SendAsync(request, cancellationToken);
            }

            ActivityTagsCollection activityTags = null;
            if (this._conf.EnableDebugMode && request.Headers.TryGetHeaderAsString(this._debugModeConfig.DebugModeHeader, out var debugModeString))
            {
                activityTags = new ActivityTagsCollection { { this._debugModeConfig.DebugModeAttribute, debugModeString } };
            }

            var routeTemplate = this._routeTemplateProvider.GetRouteTemplate(request);
            var textMapPropagator = Propagators.DefaultTextMapPropagator;
            var ctx = textMapPropagator.Extract(default, request, HttpRequestHeaderValuesGetter);

            if ((ctx.ActivityContext.TraceFlags & ActivityTraceFlags.Recorded) != 0)
            {
                Interlocked.Increment(ref this._incomingTracedRequests);
            }
            else if (ctx.ActivityContext.TraceId != default)
            {
                Interlocked.Increment(ref this._incomingNotTracedRequests);
            }
            else
            {
                Interlocked.Increment(ref this._incomingUndecidedRequests);
            }

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
                    this.EnrichActivity(activity, request, response);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void EnrichActivity(Activity activity, HttpRequestMessage request, HttpResponseMessage response)
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

            this._serverHeadersTracker.EnrichWithRequest(activity, request.Headers);
            var statusCode = 500;
            if (response != null)
            {
                statusCode = (int)response.StatusCode;
                this._serverHeadersTracker.EnrichWithResponse(activity, response.Headers);
            }

            activity.SetTag(OpenTelemetryAttributes.AttributeHttpStatusCode, statusCode);
            if (TracingUtils.IsErrorStatusCode(statusCode))
            {
                activity.SetTag("error", "true");
            }

            if (activity.GetStatus().StatusCode == StatusCode.Unset)
            {
                activity.SetStatus(TracingUtils.ResolveSpanStatusForHttpStatusCode(statusCode));
            }
        }
    }
}
