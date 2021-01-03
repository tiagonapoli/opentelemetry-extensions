namespace Napoli.OpenTelemetryExtensions.Tracing
{
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using OpenTelemetry.Trace;

    public static class TracingUtils
    {
        /// <summary>
        /// Helper method that populates span properties from http status code according
        /// to https://github.com/open-telemetry/opentelemetry-specification/blob/master/specification/trace/semantic_conventions/http.md#status.
        /// </summary>
        /// <param name="httpStatusCode">Http status code.</param>
        /// <returns>Resolved span <see cref="Status"/> for the Http status code.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Status ResolveSpanStatusForHttpStatusCode(int httpStatusCode)
        {
            return IsErrorStatusCode(httpStatusCode) ? Status.Error : Status.Unset;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsErrorStatusCode(int httpStatusCode)
        {
            return httpStatusCode < 100 || httpStatusCode > 399;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsTraceRecorded(Activity activity)
        {
            return activity != null && activity.IsAllDataRequested == true;
        }
    }
}
