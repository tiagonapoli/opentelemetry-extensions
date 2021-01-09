namespace Napoli.OpenTelemetryExtensions.QuickStart.SimpleTracingSetup
{
    using System;
    using Napoli.OpenTelemetryExtensions.Interfaces;
    using Napoli.OpenTelemetryExtensions.Tracing.DelegatingHandlers.StartTraceHandler;
    using Napoli.OpenTelemetryExtensions.Tracing.Setup;

    public class SimpleTracingSetupConfig
    {
        public IRouteTemplateProvider RouteTemplateProvider { get; set; }
        public ICustomLogger Logger { get; set; }
        public IMetricsTracker MetricsTracker { get; set; }
        public AbstractTracingComponentsConfigProvider TracingComponentsConfigProvider { get; set; }
        public InstrumentationConfig InstrumentationConfig { get; set; }
        public DebugModeConfig DebugModeConfig { get; set; }
        public TimeSpan ComponentsUpdateInterval { get; set; }
        public TimeSpan ComponentsUpdateTimeout { get; set; }
        public TimeSpan ComponentsMetricsReportInterval { get; set; }

        public void CheckValidity()
        {
            if (this.RouteTemplateProvider == null)
            {
                throw new ArgumentNullException(nameof(this.RouteTemplateProvider));
            }

            if (this.Logger == null)
            {
                throw new ArgumentNullException(nameof(this.Logger));
            }

            if (this.MetricsTracker == null)
            {
                throw new ArgumentNullException(nameof(this.MetricsTracker));
            }

            if (this.TracingComponentsConfigProvider == null)
            {
                throw new ArgumentNullException(nameof(this.TracingComponentsConfigProvider));
            }

            if (this.InstrumentationConfig == null)
            {
                throw new ArgumentNullException(nameof(this.InstrumentationConfig));
            }

            if (this.DebugModeConfig == null)
            {
                throw new ArgumentNullException(nameof(this.DebugModeConfig));
            }

            if (this.ComponentsUpdateInterval == null)
            {
                throw new ArgumentNullException(nameof(this.ComponentsUpdateInterval));
            }

            if (this.ComponentsUpdateTimeout == null)
            {
                throw new ArgumentNullException(nameof(this.ComponentsUpdateTimeout));
            }

            if (this.ComponentsMetricsReportInterval == null)
            {
                throw new ArgumentNullException(nameof(this.ComponentsMetricsReportInterval));
            }
        }
    }
}
