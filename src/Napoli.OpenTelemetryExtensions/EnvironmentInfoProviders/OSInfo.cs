namespace Napoli.OpenTelemetryExtensions.EnvironmentInfoProviders
{
    using System;
    using System.Runtime.InteropServices;
    using Napoli.OpenTelemetryExtensions.Tracing.ResourceEnhancers;

    public class OsInfo
    {
        public static string GetOsName()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return "LINUX";
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return "WINDOWS";
            }

            return RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? "OSX" : "UNKNOWN";
        }

        public static string GetOsDescription()
        {
            return Environment.OSVersion.ToString();
        }

        public static IResourceEnhancer GetResourceEnhancer()
        {
            return new OsInformation { Type = GetOsName(), Description = GetOsDescription() };
        }
    }
}
