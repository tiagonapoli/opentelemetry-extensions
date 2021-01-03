namespace Napoli.OpenTelemetryExtensions.Interfaces
{
    public interface IConfigUpdatableComponent
    {
        /// <summary>
        /// Updates the component with the current config
        /// This function doesn't have to be thread safe, the caller should
        /// guarantee just one thread executes it at a time
        /// </summary>
        /// <param name="conf"></param>
        /// <returns></returns>
        void UpdateConfiguration();

        /// <summary>
        /// Resets the component's configuration
        /// </summary>
        /// <param name="conf"></param>
        /// <returns></returns>
        void ResetConfiguration();
    }
}
