namespace Napoli.OpenTelemetryExtensions.Tracing
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;
    using Napoli.OpenTelemetryExtensions.Tracing.Conventions;
    using OpenTelemetry.Trace;

    public class Tracer
    {
        public static Tracer Singleton;

        public static void InitSingleton(ActivitySource activitySource)
        {
            Singleton = new Tracer(activitySource);
        }

        public readonly ActivitySource Instance;

        private Tracer(ActivitySource activitySource)
        {
            this.Instance = activitySource;
        }

        public async Task<T> RunAndTraceAsync<T>(string activityName, Func<Task<T>> fn)
        {
            if (!TracingUtils.IsTraceRecorded(Activity.Current)) return await fn();
            using var activity = this.Instance.StartActivity(activityName);

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

            using var activity = this.Instance.StartActivity(activityName);

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
        private static void RunAndTraceOnSuccess(Activity activity)
        {
            activity.SetStatus(Status.Ok);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void RunAndTraceOnError(Activity activity, Exception ex)
        {
            activity.SetStatus(Status.Error);
            activity.SetTag(OpenTelemetryAttributes.AttributeExceptionType, ex.GetType());
        }
    }
}
