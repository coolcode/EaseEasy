using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using log4net.Config;

namespace EaseEasy.ServiceModel.Logging {
	public class Logger : ILogger { 
		static Logger() {
			XmlConfigurator.Configure();
		} 

		private log4net.ILog _innerLogger;

		public Logger(Type type) {
			_innerLogger = log4net.LogManager.GetLogger(type);
		}

		public Logger(string name) {
			_innerLogger = log4net.LogManager.GetLogger(name);
		}

		#region ILogger Members

		public bool IsDebugEnabled {
			get { return _innerLogger.IsDebugEnabled; }
		}

		public bool IsErrorEnabled {
			get { return _innerLogger.IsErrorEnabled; }
		}

		public bool IsInfoEnabled {
			get { return _innerLogger.IsInfoEnabled; }
		}

		public bool IsWarnEnabled {
			get { return _innerLogger.IsWarnEnabled; }
		}

		public void Debug(object message) {
			_innerLogger.Debug(message);
		}

		public void Debug(object message, Exception exception) {
			_innerLogger.Debug(message, exception);
		}

		public void Error(object message) {
			_innerLogger.Error(message);
		}

		public void Error(object message, Exception exception) {
			_innerLogger.Error(message, exception);
		}

		public void Info(object message) {
			_innerLogger.Info(message);
		}

		public void Info(object message, Exception exception) {
			_innerLogger.Info(message, exception);
		}

		public void Warn(object message) {
			_innerLogger.Warn(message);
		}

		public void Warn(object message, Exception exception) {
			_innerLogger.Warn(message, exception);
		}

		private string GetStackInfo() {
			var track = new StackTrace();
			var frame = track.GetFrame(1);
			string error = string.Format("发生错误在方法：{0}, 行号：{1} , 文件：{2}",
				frame.GetMethod().Name,
				frame.GetFileLineNumber(),
				frame.GetFileName());

			return error;
		}

		#endregion
	}
}
