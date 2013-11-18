using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using log4net;
using log4net.Config;

namespace EaseEasy.WindowsService {
    public partial class Service1 : ServiceBase {
        private ILog log = LogManager.GetLogger(typeof(Service1));

        public Service1() {
            InitializeComponent();

            this.ServiceName = ServiceInfo.GetServiceName(); 
        }

        protected override void OnStart(string[] args) {
            //XmlConfigurator.Configure(new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log4net.config")));
            log.Info("服务启动");
            try {
                JobConfig.RegisterJobs();
            }
            catch (Exception ex) {
                log.Error("启动调度任务发生错误", ex);
            }
        }

        protected override void OnStop() {
            log.Info("服务停止");
        }


    }
}
