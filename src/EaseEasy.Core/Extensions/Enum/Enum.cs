using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EaseEasy {
	/// <summary>
	/// Let enum enumerable
	/// <seealso cref="http://www.cnblogs.com/EaseEasy/archive/2009/09/30/enum.html"/>
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class Enum<T> {
		public static void Foreach(Action<T> action) {
			Array values = Enum.GetValues(typeof(T));
			foreach (var value in values) {
				action((T)value);
			}
		}

		public static T FindByAttachedString(string name) {
			return Find(c => c.GetAttachedString() == name);
		}

		public static T Find(Func<T, bool> func) {
			Array values = Enum.GetValues(typeof(T));
			foreach (var value in values) {
				T t = (T)value;
				if (func(t)) {
					return t;
				}
			}
			return default(T);
		}

		public static List<T> ToList() {
			Array values = Enum.GetValues(typeof(T));
			List<T> list = new List<T>(values.Length);
			foreach (var value in values) {
				list.Add((T)value);
			}
			return list;
		}

		public static IEnumerable<T> AsEnumerable() {
			return new EnumQuery<T>();
		}

		public static IEnumerable<TResult> Select<TResult>(Func<T, TResult> selector) {
			return ToList().Select(selector);
		}

		public static IEnumerable<T> Where(Func<T, bool> predicate) {
			return AsEnumerable().Where(predicate);
		}

		#region EnumQuery
		class EnumQuery<TEnum> : IEnumerable<TEnum> {
			private readonly static IEnumerable<TEnum> innerList;

			static EnumQuery() {
				Array values = Enum.GetValues(typeof(T));
				innerList = values.Cast<TEnum>();
			}

			#region IEnumerable<TEnum> Members

			public IEnumerator<TEnum> GetEnumerator() {
				return innerList.GetEnumerator();
			}

			#endregion

			#region IEnumerable Members

			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
				return GetEnumerator();
			}

			#endregion
		}
		#endregion
	}
}
