namespace Napoli.OpenTelemetryExtensions.Utils
{
    using System;
    using System.Linq;
    using System.Net.Http.Headers;
    using System.Runtime.CompilerServices;

    public static class HttpUtilitiesExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetHeaderAsString(this HttpHeaders headers, string name, out string content)
        {
            if (!headers.TryGetValues(name, out var headerContentEnumerable))
            {
                content = null;
                return false;
            }

            var contentEnumerable = headerContentEnumerable as string[] ?? headerContentEnumerable.ToArray();
            content = contentEnumerable.Length == 1
                ? contentEnumerable[0]
                : String.Join(",", contentEnumerable);

            return true;
        }
    }
}
