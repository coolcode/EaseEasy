using System;
using System.Collections;
using System.Web.Caching; 

namespace EaseEasy.Caching {
	public interface ICache : ICacheStrategy, IEnumerable {
		object this[string key] { get; set; }
		void Insert(string key, object value, CacheDependency dependency);
		void Insert(string key, object value, DateTime absoluteExpiration);
		void Insert(string key, object value, int minutes);
	}
}
