using System;
using System.Collections.Generic;
using System.Linq;

namespace EaseEasy {
	public static class ElementExtensions {
		public static bool InRange<T>(this IComparable<T> t, T minT, T maxT) {
			return t.CompareTo(minT) >= 0 && t.CompareTo(maxT) <= 0;
		}

		public static bool InRange(this IComparable t, object minT, object maxT) {
			return t.CompareTo(minT) >= 0 && t.CompareTo(maxT) <= 0;
		}

		public static void ForEach(this int count, Action<int> action) {
			for (int i = 0; i < count; i++)
				action(i);
		}

		public static IEnumerable<T> Select<T>(this int count, Func<int, T> selector) {
			for (int i = 0; i < count; i++)
				yield return selector(i);
		}
	}
}
