using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace CoolCode.Reflection {
	public sealed class FastPropertyInfo {
		public string Name { get { return this.Property.Name; } }

		public PropertyInfo Property { get; private set; }

		private FastInvokeHandler getHandler;
		private FastInvokeHandler setHandler;

		public FastPropertyInfo(PropertyInfo property) {
			Property = property;
		}

		public object Get(object instanse) {
			if (getHandler == null) {
				getHandler = Property.GetGetMethod().GetFastInvoker();
			}
			return getHandler(instanse, null);
		}

		public void Set(object instanse, object value) {
			if (setHandler == null) {
				setHandler = Property.GetSetMethod().GetFastInvoker();
			}
			setHandler(instanse, value);
		}
	}
}
