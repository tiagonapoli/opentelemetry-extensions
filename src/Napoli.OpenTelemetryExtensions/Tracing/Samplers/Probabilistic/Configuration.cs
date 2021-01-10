namespace Napoli.OpenTelemetryExtensions.Tracing.Samplers.Probabilistic
{
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    public class Configuration
    {
        public static Configuration GetDefault()
        {
            return new Configuration(0, new Dictionary<string, double>());
        }

        private readonly Dictionary<string, KeyValuePair<double, long>> _perOperationIdUpperBound;
        private readonly KeyValuePair<double, long> _defaultIdUpperBound;

        public Configuration(double defaultProbability, Dictionary<string, double> perOperationProbability)
        {

            this._defaultIdUpperBound = new KeyValuePair<double, long>(defaultProbability, CalculateIdUpperBound(defaultProbability));

            this._perOperationIdUpperBound = new Dictionary<string, KeyValuePair<double, long>>();

            if (perOperationProbability == null)
            {
                return;
            }

            foreach (var item in perOperationProbability)
            {
                this._perOperationIdUpperBound[item.Key] = new KeyValuePair<double, long>(item.Value, CalculateIdUpperBound(item.Value));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public KeyValuePair<double, long> GetSamplingConfig(string operation)
        {
            return this._perOperationIdUpperBound.TryGetValue(operation, out var ret) ? ret : this._defaultIdUpperBound;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long CalculateIdUpperBound(double probability)
        {
            if (probability <= 0.0)
            {
                return long.MinValue;
            }

            if (probability >= 1.0)
            {
                return long.MaxValue;
            }

            return (long)(probability * long.MaxValue);
        }
    }
}
