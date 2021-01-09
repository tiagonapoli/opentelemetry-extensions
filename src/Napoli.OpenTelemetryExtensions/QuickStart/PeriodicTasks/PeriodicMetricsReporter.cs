namespace Napoli.OpenTelemetryExtensions.QuickStart.PeriodicTasks
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Napoli.OpenTelemetryExtensions.Interfaces;

    public class PeriodicMetricsReporter : PeriodicTask
    {
        private const string ReportMetricsOperationName = "ReportMetrics";
        private readonly IEnumerable<IMeasurableComponent> _measurableComponents;
        private readonly IMetricsTracker _metricsTracker;
        private readonly ICustomLogger _logger;

        public PeriodicMetricsReporter(TimeSpan interval, IEnumerable<IMeasurableComponent> measurableComponents,
            ICustomLogger logger, IMetricsTracker metricsTracker) : base(interval)
        {
            this._measurableComponents = measurableComponents;
            this._logger = logger;
            this._metricsTracker = metricsTracker;
        }

        protected override Task Run()
        {
            try
            {
                foreach (var el in this._measurableComponents)
                {
                    el.ReportMetrics(this._metricsTracker);
                }
            }
            catch (Exception ex)
            {
                this._logger.Error(ReportMetricsOperationName, $"MetricsReportError", ex);
            }

            return Task.FromResult(0);
        }
    }
}
