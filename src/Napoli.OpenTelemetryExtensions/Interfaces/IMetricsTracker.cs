namespace Napoli.OpenTelemetryExtensions.Interfaces
{
    using System.Collections.Generic;

    public interface IMetricsTracker
    {
        void Register(string metricName, long value, Dictionary<string, string> labels = null);
    }
}
