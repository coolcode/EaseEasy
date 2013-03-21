using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EaseEasy.Caching {
	public static class CacheExtension {
		public static T Get<T>(this ICacheStrategy cache, string key, Func<T> provideData) {
			object obj = cache.Get(key);
			if (obj == null) {
				T t = provideData();
				cache.Insert(key, t);
				return t;
			}
			return (T)obj;
		}
	}
}
