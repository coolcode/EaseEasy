using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EaseEasy.ServiceModel.Logging {
	public interface ILogger {
		void Debug(object message);
		void Debug(object message, Exception exception);

		void Error(object message);
		void Error(object message, Exception exception);

		void Info(object message);
		void Info(object message, Exception exception);

		void Warn(object message);
		void Warn(object message, Exception exception);
	}
}
