﻿using System.Web.Mvc;

namespace EaseEasy.Web.Mvc {
	public static class ActionResultExtensions {
		public static XmlResult Xml<T>(this Controller controller, T model) {
			return new XmlResult(model);
		}

		public static MessageResult Message(this Controller controller, bool success = true, string message = null) {
			return new MessageResult(success, message);
		}

		public static MessageResult Success(this Controller controller, object result = null) {
			return new MessageResult(true, result);
		}

		public static MessageResult Fail(this Controller controller, string message) {
			return new MessageResult(false, message);
		}

        public static MessageResult Fail(this Controller controller, object result = null) {
            return new MessageResult(false, result);
        }
	}
}
