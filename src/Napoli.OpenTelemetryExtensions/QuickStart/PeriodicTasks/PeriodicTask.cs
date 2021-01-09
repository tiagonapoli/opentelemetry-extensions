namespace Napoli.OpenTelemetryExtensions.QuickStart.PeriodicTasks
{
    using System;
    using System.Threading.Tasks;
    using System.Timers;

    public abstract class PeriodicTask
    {
        private readonly Timer _timer;

        protected PeriodicTask(TimeSpan interval)
        {
            this._timer = new Timer { AutoReset = false, Enabled = false, Interval = interval.TotalMilliseconds };
            this._timer.Elapsed += this.TimerCallback;
        }

        private async void TimerCallback(object source, ElapsedEventArgs e)
        {
            try
            {
                await this.Run();
            }
            finally
            {
                this.Start();
            }
        }

        public void Start()
        {
            this._timer.Start();
        }

        protected abstract Task Run();
    }
}
