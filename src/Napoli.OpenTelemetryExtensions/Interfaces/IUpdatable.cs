namespace Napoli.OpenTelemetryExtensions.Interfaces
{
    using System.Threading;
    using System.Threading.Tasks;

    public interface IUpdatable
    {
        Task UpdateAsync(CancellationToken cancellationToken);
    }
}
