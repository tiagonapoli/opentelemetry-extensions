namespace Napoli.OpenTelemetryExtensions.Interfaces
{
    using System;

    public interface ICustomLogger
    {
        void Info(string id, string message);
        void Error(string id, string message, Exception ex);
    }
}
