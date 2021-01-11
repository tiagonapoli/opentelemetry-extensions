namespace Napoli.OpenTelemetryExtensions.QuickStart.SimpleTracingSetup
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Net.Http;
    using Napoli.OpenTelemetryExtensions.EnvironmentInfoProviders;
    using Napoli.OpenTelemetryExtensions.EnvironmentInfoProviders.AWS;
    using Napoli.OpenTelemetryExtensions.Interfaces;
    using Napoli.OpenTelemetryExtensions.QuickStart.PeriodicTasks;
    using Napoli.OpenTelemetryExtensions.Tracing;
    using Napoli.OpenTelemetryExtensions.Tracing.DelegatingHandlers.StartTraceHandler;
    using Napoli.OpenTelemetryExtensions.Tracing.ResourceEnhancers;
    using Napoli.OpenTelemetryExtensions.Tracing.Samplers.DebugMode;
    using Napoli.OpenTelemetryExtensions.Tracing.Samplers.Probabilistic;
    using Napoli.OpenTelemetryExtensions.Tracing.Samplers.TracesThrottler;
    using Napoli.OpenTelemetryExtensions.Tracing.Setup;
    using OpenTelemetry.Trace;

    public class SimpleTracingSetup : IDisposable
    {
        public static SimpleTracingSetup Instance;

        public static void InitSingleton(SimpleTracingSetupConfig setupConfig)
        {
            if (Instance != null)
            {
                return;
            }

            Instance = new SimpleTracingSetup(setupConfig);
        }

        private readonly StartTraceHandler _startTraceHandler;
        private readonly PeriodicUpdater _periodicUpdater;
        private readonly PeriodicMetricsReporter _periodicMetricsReporter;
        private readonly SimpleTracingSetupConfig _setupConfig;
        private bool _configured;
        private TracerProvider _tracerProvider;

        /// <summary>
        /// Should be instantiated before any Http request takes place
        /// </summary>
        /// <param name="setupConfig"></param>
        private SimpleTracingSetup(SimpleTracingSetupConfig setupConfig)
        {
            setupConfig.CheckValidity();
            this._setupConfig = setupConfig;

            var probabilisticSampler = new ProbabilisticSampler(this._setupConfig.TracingComponentsConfigProvider);
            var debugModeSampler = new DebugModeSampler(probabilisticSampler, this._setupConfig.DebugModeConfig);
            var sampler = new TracesThrottlerSampler(
                new ParentBasedSampler(debugModeSampler),
                this._setupConfig.TracingComponentsConfigProvider);

            this._setupConfig.InstrumentationConfig.WithSampler(sampler);

            TracingSetup.PreConfigure(this._setupConfig.InstrumentationConfig);

            this._startTraceHandler = new StartTraceHandler(
                ActivityTracer.Singleton,
                () => sampler.DecrementOngoingTraces(),
                this._setupConfig.RouteTemplateProvider, this._setupConfig.DebugModeConfig,
                this._setupConfig.TracingComponentsConfigProvider
            );

            this._periodicUpdater = new PeriodicUpdater(
                this._setupConfig.ComponentsUpdateInterval,
                this._setupConfig.ComponentsUpdateTimeout,
                this._setupConfig.TracingComponentsConfigProvider,
                new List<IConfigUpdatableComponent> { probabilisticSampler, sampler, this._startTraceHandler },
                this._setupConfig.Logger, this._setupConfig.MetricsTracker);

            this._periodicMetricsReporter = new PeriodicMetricsReporter(
                this._setupConfig.ComponentsMetricsReportInterval,
                new List<IMeasurableComponent> { sampler, this._startTraceHandler },
                this._setupConfig.Logger,
                this._setupConfig.MetricsTracker);
        }

        public void ConfigureTracing()
        {
            try
            {
                this._setupConfig.InstrumentationConfig.AddResourceEnhancers(new List<IResourceEnhancer>
                {
                    OsInfo.GetResourceEnhancer(),
                    AwsInstanceIdentityProvider.GetCloudResourceEnhancer(),
                    AwsInstanceIdentityProvider.GetHostResourceEnhancer()
                });

                this._tracerProvider = TracingSetup.Configure(this._setupConfig.InstrumentationConfig);
                this._periodicUpdater.Start();
                this._periodicMetricsReporter.Start();
                this._configured = true;
            }
            catch (Exception ex)
            {
                this._setupConfig.Logger.Error(nameof(SimpleTracingSetup), "Failed to configure OpenTelemetry", ex);
            }
        }

        public void RegisterTracingDelegatingHandler(Collection<DelegatingHandler> messageHandlers)
        {
            if (this._configured)
            {
                messageHandlers.Add(this._startTraceHandler);
            }
        }

        public void Dispose()
        {
            this._tracerProvider.Shutdown(2000);
        }
    }
}
