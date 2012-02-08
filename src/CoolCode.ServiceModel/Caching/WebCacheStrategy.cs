using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Caching;
using CoolCode.Caching;

namespace CoolCode.Caching {
	public class WebCacheStrategy : ICache {
		private readonly Cache _cache = System.Web.HttpContext.Current.Cache;

		#region ICacheStrategy Members

		public WebCacheStrategy() {
		}

		public void Insert(string key, object value) {
			_cache.Insert(key, value);
		}

		public object Get(string key) {
			return _cache.Get(key);
		}

		public void Remove(string key) {
			_cache.Remove(key);
		}

		#endregion

		#region ICache Members

		public object this[string key] {
			get {
				return _cache[key];
			}
			set {
				_cache[key] = value;
			}
		}
		 
		public void Insert(string key, object value, CacheDependency dependency) {
			_cache.Insert(key, value, dependency);
		}

		public void Insert(string key, object value, DateTime absoluteExpiration) {
			_cache.Insert(key, value, null, absoluteExpiration, Cache.NoSlidingExpiration);
		}

		public void Insert(string key, object value, int minutes) {
			_cache.Insert(key, value, null, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(minutes));
		}

		#endregion

		#region IEnumerable Members

		public System.Collections.IEnumerator GetEnumerator() {
			return _cache.GetEnumerator();
		}

		#endregion
	}

}
