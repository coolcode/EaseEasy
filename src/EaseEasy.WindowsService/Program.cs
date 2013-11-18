using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using log4net.Config;
using System.Reflection;

namespace EaseEasy.WindowsService {
    static class Program {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main() {
            try {
               // string configPath = Assembly.GetExecutingAssembly().Location;
                XmlConfigurator.Configure(new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log4net.config")));
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[]
                                    {
                                        new Service1()
                                    };
                ServiceBase.Run(ServicesToRun);
            }
            catch (Exception ex) {
                File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "error.log"), ex.Message + "\r\nStack:" + ex.StackTrace);
            }
        }
    }
}
