using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Caching;

namespace CoolCode.Caching {
	public static class CacheExtensions {
		public static T Get<T>(this ICache cache, string key, int minutes  , Func<T> provideData) {
			return cache.Get(key, provideData, t => cache.Insert(key, t, minutes));
		}

		public static T Get<T>(this ICache cache, string key, CacheDependency dependency, Func<T> provideData) {
			return cache.Get(key, provideData, t => cache.Insert(key, t, dependency));
		}

		private static T Get<T>(this ICache cache, string key, Func<T> provideData, Action<T> storeDataToCache) {
			return CacheHelper.Get(() => cache.Get(key), provideData, storeDataToCache);
		}
	}
}
