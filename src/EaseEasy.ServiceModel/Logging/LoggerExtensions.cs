using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EaseEasy.ServiceModel.Logging {
	public static class LoggerExtensions {
		public static void Debug(this ILogger logger, string format, params object[] args) {
			logger.Debug(string.Format(format, args));
		}

		public static void Error(this ILogger logger, string format, params object[] args) {
			logger.Error(string.Format(format, args));
		}

		public static void Info(this ILogger logger, string format, params object[] args) {
			logger.Info(string.Format(format, args));
		}

		public static void Warn(this ILogger logger, string format, params object[] args) {
			logger.Warn(string.Format(format, args));
		}
	}
}
