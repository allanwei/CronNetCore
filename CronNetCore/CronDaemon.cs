﻿using System;
using System.Collections.Generic;
using System.Timers;

namespace CronNetCore
{
    public interface ICronDaemon
    {
        void AddJob(string schedule, Action action);
        void Start();
        void Stop();
    }
    public class CronDaemon : ICronDaemon
    {
        private readonly System.Timers.Timer timer = new System.Timers.Timer(30000);
        private readonly List<ICronJob> cron_jobs = new List<ICronJob>();
        private DateTime _last = DateTime.Now;

        public CronDaemon()
        {
            timer.AutoReset = true;
            timer.Elapsed += Timer_elapsed;
        }

        public void AddJob(string schedule, Action action)
        {
            var cj = new CronJob(schedule, action);
            cron_jobs.Add(cj);
        }

        public void Start()
        {
            timer.Start();
        }

        public void Stop()
        {
            timer.Stop();

            foreach (CronJob job in cron_jobs)
                job.Abort();
        }

        private void Timer_elapsed(object sender, ElapsedEventArgs e)
        {
            if (DateTime.Now.Minute != _last.Minute)
            {
                _last = DateTime.Now;
                foreach (ICronJob job in cron_jobs)
                    job.Execute(DateTime.Now);
            }
        }
    }
}
