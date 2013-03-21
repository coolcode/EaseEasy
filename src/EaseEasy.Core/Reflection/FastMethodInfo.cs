using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace EaseEasy.Reflection {
	public sealed class FastMethodInfo {
		public MethodInfo method;

		private FastInvokeHandler callHandler;

		public FastMethodInfo(MethodInfo method) {
			this.method = method;
		}

		public object Call(object instanse, params object[] parameters) {
			if (callHandler == null) {
				callHandler = method.GetFastInvoker();
			}
			return callHandler(instanse, parameters);
		}
	}
}
