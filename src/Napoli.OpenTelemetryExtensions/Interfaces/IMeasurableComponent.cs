namespace Napoli.OpenTelemetryExtensions.Interfaces
{
    public interface IMeasurableComponent
    {
        /// <summary>
        /// Report the component's metrics
        /// </summary>
        /// <returns></returns>
        void ReportMetrics(IMetricsTracker metricsTracker);
    }
}
