using System;
using log4net;
using Quartz;
using Quartz.Impl;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace EaseEasy.WindowsService {
    public static class JobConfig {
        private static readonly ILog logger = LogManager.GetLogger(typeof(JobConfig));

        public static void RegisterJobs() {
            // construct a scheduler factory
            ISchedulerFactory schedFact = new StdSchedulerFactory();

            // get a scheduler
            var sched = schedFact.GetScheduler();

            var triggerJobs = new Dictionary<IJobDetail, IList<ITrigger>>();

            foreach (var jobItem in GetJobConfig()) {
                logger.Debug(string.Format("Begin Load Job[{0}] - StartAt:{1}, Schedule Interval:{2}", 
                    jobItem.Name, 
                    string.Join(",",jobItem.StartAt), 
                    jobItem.Schedule)
                );

                if (!jobItem.Enable) {
                    logger.Debug(string.Format("End Load Job[{0}] - Disabled! ", jobItem.Name));
                    continue;
                }
                // construct job info
                var jobDetail = JobBuilder.Create()
                    .WithIdentity(jobItem.Name, "jobs")
                    .OfType(typeof(CommonJob))
                    .WithDescription(jobItem.Descritpion)
                    .UsingJobData("type", jobItem.Type)
                    .UsingJobData("method", jobItem.Method)
                    .Build();

                // trigger
                var triggers = new List<ITrigger>();

                foreach (var startAt in jobItem.StartAt) {
                    var trigger = TriggerBuilder.Create()
                        .ForJob(jobDetail)
                        .StartAt(startAt)
                        .WithSimpleSchedule(x => x.WithIntervalInSeconds(jobItem.Schedule).RepeatForever())
                        .Build();

                    triggers.Add(trigger);
                    //sched.ScheduleJob(jobDetail, trigger);
                   // sched.ScheduleJob(trigger);
                }

                triggerJobs.Add(jobDetail, triggers);
                logger.Debug(string.Format("End Load Job[{0}] ", jobItem.Name));
            }
            sched.ScheduleJobs(triggerJobs,false);
            sched.Start();
            /*
            // construct job info
            var jobDetail = JobBuilder.Create()
                .WithIdentity("syncJob", "jobs")
                .OfType(typeof(CommonJob))
                .WithDescription("同步数据")
                .UsingJobData("type", "EaseEasy.PBC.Services.WinService,EaseEasy.PBC.Services")
                .UsingJobData("method", "Execute")
                .Build();

            // fire every 5 mins
            var trigger = TriggerBuilder.Create()
                .ForJob(jobDetail)
                .StartAt(DateTimeOffset.Now.AddSeconds(10))
                .WithSimpleSchedule(x => x.WithIntervalInHours(1).RepeatForever())
                .Build();

            sched.ScheduleJob(jobDetail, trigger);
            */
        }

        public static IEnumerable<JobEntity> GetJobConfig() {
            var jobConfigFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "job.config");
            var jobConfigs = new List<JobEntity>();
            var xml = XElement.Load(jobConfigFileName);
            var q = from x in xml.Elements("job")
                    select new JobEntity {
                        Name = x.Value("name"),
                        Descritpion = x.Value("description"),
                        Type = x.Value("type"),
                        Method = x.Value("method", "Execute"),
                        RunMode = x.Value("runMode", t => (RunMode)Enum.Parse(typeof(RunMode), t, true)),
                        Enable = x.Value("enable", Convert.ToBoolean,true),
                        Schedule = x.Value("schedule", ParseTimeSpan),
                        StartAt = x.Value("startAt", ParseTimeOffsets, new[]{DateTimeOffset.Now.AddSeconds(10)})
                    };

            return q;
        }
        
        private static DateTimeOffset[] ParseTimeOffsets(string p) {
            var ps = p.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            return ps.Select(ParseTimeOffset).ToArray();
        }

        private static DateTimeOffset ParseTimeOffset(string p) {
            if (string.Equals(p,"now", StringComparison.CurrentCultureIgnoreCase)) {
                return DateTimeOffset.Now.AddSeconds(10);
            }

            var d = DateTimeOffset.Parse(p);
            if (d < DateTimeOffset.Now && d.Date == DateTime.Today) {
                d = d.AddDays(1);
            }

            return d;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="timespanText">1day=60*60*24</param>
        /// <returns></returns>
        private static int ParseTimeSpan(string timespanText) {
            var ts = from t in timespanText.Split('*') select int.Parse(t);
            var r = 1;
            foreach (var t in ts) {
                r *= t;
            }
            return r;
        }

        public class JobEntity {
            public string Name { get; set; }
            public string Descritpion { get; set; }
            public string Type { get; set; }
            public string Method { get; set; }
            public RunMode RunMode { get; set; }
            public bool Enable { get; set; }
            public int Schedule { get; set; }
            public DateTimeOffset[] StartAt { get; set; }

            public JobEntity() {
                StartAt = new []{DateTimeOffset.Now};
            }
        }

        public enum RunMode {
            Singleton,
            Multi
        }
    }

    public class CommonJob : IJob {
        private static readonly ILog logger = LogManager.GetLogger(typeof(CommonJob));

        public void Execute(IJobExecutionContext context) {
            var map = context.JobDetail.JobDataMap;
            var typeName = (string)map["type"];
            var methodName = (string)map["method"];

            logger.Info("开始执行:" + context.JobDetail.Description);

            try {
                var type = Type.GetType(typeName);
                var method = type.GetMethod(methodName,Type.EmptyTypes);
                method.Invoke(Activator.CreateInstance(type), null);
            }
            catch (Exception ex) {
                logger.Error("执行任务发生异常，Job信息：[" + methodName + ":" + typeName + "]", ex);
            }
            finally {
                logger.Info("释放内存:" + context.JobDetail.Description);
                GC.Collect();
                logger.Info("结束执行:" + context.JobDetail.Description);
            }
        }
    }
}