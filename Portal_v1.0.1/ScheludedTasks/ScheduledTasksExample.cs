using Quartz;
using Quartz.Impl;
using System;
using System.Configuration;

namespace ScheduledTasks.ScheduledTaskExample
{
    public class JobScheduler
    {
        public static void Start()
        {
            IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();
            scheduler.Start();
            var scheludeValue = "0 " + Convert.ToString(ConfigurationManager.AppSettings["Minute"]) +" " + Convert.ToString(ConfigurationManager.AppSettings["Hour"]) + " " + "? * "+ Convert.ToString(ConfigurationManager.AppSettings["DayOfWeek"]);
            Convert.ToString(ConfigurationManager.AppSettings["Hour"]);

            IJobDetail job = JobBuilder.Create<ScheduledTasks.EmailTest.EmailJob>().Build();

            ITrigger trigger = TriggerBuilder.Create()
            .WithIdentity("trigger1", "group1")
            .WithSchedule(CronScheduleBuilder.CronSchedule(scheludeValue)).Build();

            scheduler.ScheduleJob(job, trigger);
        }
    }
}