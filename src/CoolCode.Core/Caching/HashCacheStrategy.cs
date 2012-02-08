using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace CoolCode.Caching {
	public class HashCacheStrategy : ICacheStrategy {
		private readonly Hashtable _objectStore = new Hashtable();

		public HashCacheStrategy() {
		}

		public void Insert(string key, object obj) {
			_objectStore.Add(key, obj);
		}

		public object Get(string key) {
			return _objectStore[key];
		}

		public void Remove(string key) {
			_objectStore.Remove(key);
		}

		#region IEnumerable Members

		IEnumerator IEnumerable.GetEnumerator() {
			return _objectStore.GetEnumerator();
		}

		#endregion
	}
}
