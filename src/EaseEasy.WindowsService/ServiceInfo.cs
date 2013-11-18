using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;

namespace EaseEasy.WindowsService {
    internal class ServiceInfo {
        private static readonly ILog log = LogManager.GetLogger(typeof(ServiceInfo));

        public static string GetServiceName() {
            return GetConfig("ServiceName", typeof(ServiceInfo).Assembly.GetName().Name);
        }

        public static string GetServiceDisplayName() {
            return GetConfig("ServiceDisplayName", "统一后台服务");
        }

        public static string GetServiceDescription() {
            return GetConfig("ServiceDescription", "统一后台服务-所有服务以插件形式运行");
        }

        internal static string GetConfig(string key, string defaultValue) {
            try {
                string configValue = ConfigurationManager.AppSettings[key];
                if (string.IsNullOrWhiteSpace(configValue)) {
                    string configPath = Assembly.GetExecutingAssembly().Location;
                    Configuration config = ConfigurationManager.OpenExeConfiguration(configPath);
                    configValue = config.AppSettings.Settings[key].Value;
                }

                return string.IsNullOrWhiteSpace(configValue) ? defaultValue : configValue;
            }
            catch (Exception ex) {
                log.Error("获取配置信息发生错误", ex);

                return defaultValue;
            }
        }
    }
}
