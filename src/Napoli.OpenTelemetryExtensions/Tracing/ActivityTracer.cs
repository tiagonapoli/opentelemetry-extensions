namespace Napoli.OpenTelemetryExtensions.Tracing
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;
    using Napoli.OpenTelemetryExtensions.Tracing.Conventions;
    using OpenTelemetry.Context.Propagation;
    using OpenTelemetry.Trace;

    public class ActivityTracer
    {
        public static ActivityTracer Singleton;

        public static void InitSingleton(ActivitySource activitySource)
        {
            Singleton = new ActivityTracer(activitySource);
        }

        public readonly ActivitySource ActivitySource;

        private ActivityTracer(ActivitySource activitySource)
        {
            this.ActivitySource = activitySource;
        }

        public async Task<T> RunAndTraceAsync<T>(string activityName, Func<Task<T>> fn)
        {
            if (!TracingUtils.IsTraceRecorded(Activity.Current)) return await fn();
            using var activity = this.ActivitySource.StartActivity(activityName);

            try
            {
                var res = await fn();
                RunAndTraceOnSuccess(activity);
                return res;
            }
            catch (Exception ex)
            {
                RunAndTraceOnError(activity, ex);
                throw;
            }
        }

        public async Task RunAndTraceAsync(string activityName, Func<Task> fn)
        {
            if (!TracingUtils.IsTraceRecorded(Activity.Current))
            {
                await fn();
                return;
            }

            using var activity = this.ActivitySource.StartActivity(activityName);

            try
            {
                await fn();
                RunAndTraceOnSuccess(activity);
            }
            catch (Exception ex)
            {
                RunAndTraceOnError(activity, ex);
                throw;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Activity StartEntrypointActivity(string name, ActivityKind kind, PropagationContext parentContext, IEnumerable<KeyValuePair<string, object>> tags = null, IEnumerable<ActivityLink> links = null, DateTimeOffset startTime = default)
        {
            return this.ActivitySource.StartActivity(name, kind, parentContext.ActivityContext, tags, links, startTime) ?? CreateDummyUnrecordedActivity(parentContext);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Activity CreateDummyUnrecordedActivity(PropagationContext ctx)
        {
            var activity = new Activity("unrecorded-activity");
            activity.SetParentId(ctx.ActivityContext.TraceId, ctx.ActivityContext.SpanId, ctx.ActivityContext.TraceFlags);
            activity.TraceStateString = ctx.ActivityContext.TraceState;
            activity.Start();
            activity.IsAllDataRequested = false;
            return activity;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void RunAndTraceOnSuccess(Activity activity)
        {
            activity.SetStatus(Status.Ok);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void RunAndTraceOnError(Activity activity, Exception ex)
        {
            activity.SetStatus(Status.Error);
            activity.SetTag("error", "true");
            activity.SetTag(OpenTelemetryAttributes.AttributeExceptionType, ex.GetType());
        }
    }
}
