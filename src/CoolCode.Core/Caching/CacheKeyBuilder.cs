using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoolCode.Caching {
	/// <summary>
	/// 用于将若干对象/字符串组合成缓存的Key
	/// </summary>
	public class CacheKeyBuilder {
		public static string Join(params string[] keys) {
			return keys.Aggregate((a, b) => (a ?? "[Null]") + "_" + (b ?? "[Null]"));
		}

		public static string Join(params object[] keys) {
			return keys.Aggregate((a, b) => ((a ?? "[Null]") + "_" + (b ?? "[Null]"))).ToString();
		}
	}
}
