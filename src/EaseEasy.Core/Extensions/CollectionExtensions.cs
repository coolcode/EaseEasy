using System;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace EaseEasy {
	public static class CollectionExtensions {
		public static bool AddIfNotExists<TKey, TValue>(this IDictionary<TKey, TValue> source, TKey key, TValue value) {
			if (!source.ContainsKey(key)) {
				source[key] = value;
				return true;
			}
			return false;
		}

		public static bool AddIfNotExists<TKey, TValue>(this IDictionary<TKey, TValue> source, TKey key, Func<TValue> provideValue) {
			if (!source.ContainsKey(key)) {
				source[key] = provideValue();
				return true;
			}
			return false;
		}

		public static TValue GetValue<TKey, TValue>(this  IDictionary<TKey, TValue> source, TKey key, Func<TValue> provideValue) {
			if (source.ContainsKey(key))
				return source[key];
			return provideValue();
		}

		public static TValue GetValue<TKey, TValue>(this IDictionary<TKey, TValue> source, TKey key, TValue defaultValue) {
			return source.ContainsKey(key) ? source[key] : defaultValue;
		}

		public static string GetValue(this NameValueCollection collection, string name, string defaultValue) {
			string configValue = collection[name];
			return (string.IsNullOrEmpty(configValue)) ? defaultValue : configValue;
		}

		public static void ForEach<T>(this IEnumerable<T> source, Action<T> action) {
			foreach (T element in source)
				action(element);
		}

		public static void ForEach<T>(this IEnumerable<T> source, Action<T, int> action) {
			int i = 0;
			foreach (T element in source)
				action(element, i++);
		}

		public static string Join(this IEnumerable<string> source, string separator) {
			return string.Join(separator, source.ToArray());
		}

	}
}
