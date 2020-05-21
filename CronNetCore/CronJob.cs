using System;
using System.Threading;
using System.Threading.Tasks;

namespace CronNetCore
{
    public interface ICronJob
    {
        void Execute(DateTime date_time);
        void Abort();
    }
    public class CronJob : ICronJob
    {
        private readonly ICronSchedule _cron_schedule = new CronSchedule();
        private readonly Action _action;
        private Task _thread;
        private readonly CancellationTokenSource cancellationToken;

        public CronJob(string schedule, Action action)
        {
            _cron_schedule = new CronSchedule(schedule);
            _action = action;
            cancellationToken = new CancellationTokenSource();
            //_thread = new Thread(thread_start);
        }

        private object _lock = new object();
        public void Execute(DateTime date_time)
        {
            lock (_lock)
            {
                if (!_cron_schedule.IsTime(date_time))
                    return;
                if (_thread != null)
                {
                    if (!_thread.IsCompleted)
                    {
                        return;
                    }
                }

                _thread = new Task(_action,cancellationToken.Token);
                _thread.Start();
                _thread.Wait();
                _thread.Dispose();
                _thread = null;
               
             
            }
        }

        public void Abort()
        {
            cancellationToken.Cancel();
        }

    }
}
