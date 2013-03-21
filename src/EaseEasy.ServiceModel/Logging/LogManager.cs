using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace EaseEasy.ServiceModel.Logging {
	public class LogManager {

		public static ILogger Current {
			get {
				StackFrame frame = new StackFrame(1, false);
				return GetLogger(frame.GetMethod().DeclaringType);
			}
		}

		public static ILogger GetLogger(Type type) {
			return new Logger(type);
		}

		public static ILogger GetLogger(string name) {
			return new Logger(name);
		}

		/*
		public static void Config(string configFile) {
			log4net.Config.XmlConfigurator.ConfigureAndWatch(new System.IO.FileInfo(configFile));
		}

		public static void Config() {
			log4net.Config.XmlConfigurator.Configure();
		}*/
	}
}
