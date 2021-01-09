namespace Napoli.OpenTelemetryExtensions.QuickStart.PeriodicTasks
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;
    using Napoli.OpenTelemetryExtensions.Interfaces;

    public class PeriodicUpdater : PeriodicTask
    {
        private const string UpdateConfigOperationName = "UpdateConfiguration";
        private static readonly string UpdateConfigLatencyMetric = $"{UpdateConfigOperationName}.Latency";

        private readonly IMetricsTracker _metricsTracker;
        private readonly ICustomLogger _logger;
        private readonly IUpdatable _configProvider;
        private readonly IEnumerable<IConfigUpdatableComponent> _updatableComponents;
        private readonly int _updateTimeoutMs;

        public PeriodicUpdater(TimeSpan interval, TimeSpan updateTimeout,
            IUpdatable configProvider, IEnumerable<IConfigUpdatableComponent> updatableComponents,
            ICustomLogger logger, IMetricsTracker metricsTracker) : base(interval)
        {
            this._updateTimeoutMs = (int)updateTimeout.TotalMilliseconds;
            this._configProvider = configProvider;
            this._updatableComponents = updatableComponents;
            this._logger = logger;
            this._metricsTracker = metricsTracker;
        }

        protected override async Task Run()
        {
            using (var cancellationTokenSource = new CancellationTokenSource(this._updateTimeoutMs))
            {
                var stopwatch = Stopwatch.StartNew();
                try
                {
                    await this._configProvider.UpdateAsync(cancellationTokenSource.Token);
                    if (this.UpdateComponents() == false)
                    {
                        this.ResetComponents();
                    }

                    this.EndUpdate(stopwatch);
                }
                catch (Exception ex)
                {
                    this.EndUpdate(stopwatch, ex);
                }
            }
        }

        private bool UpdateComponents()
        {
            try
            {
                foreach (var el in this._updatableComponents)
                {
                    el.UpdateConfiguration();
                }

                return true;
            }
            catch (Exception ex)
            {
                this._logger.Error(UpdateConfigOperationName, "UpdateError", ex);
            }

            return false;
        }

        private void ResetComponents()
        {
            try
            {
                foreach (var el in this._updatableComponents)
                {
                    el.ResetConfiguration();
                }
            }
            catch (Exception ex)
            {
                this._logger.Error(UpdateConfigOperationName, "ResetError", ex);
            }
        }

        private void EndUpdate(Stopwatch stopwatch, Exception ex = null)
        {
            stopwatch.Stop();
            if (ex == null)
            {
                this._metricsTracker.Register(UpdateConfigLatencyMetric, stopwatch.ElapsedMilliseconds);
            }
            else
            {
                this._metricsTracker.Register(UpdateConfigLatencyMetric, stopwatch.ElapsedMilliseconds,
                    new Dictionary<string, string> { { "error", "true" } });
                this._logger.Error(UpdateConfigOperationName, $"UpdateError", ex);
            }
        }
    }
}
