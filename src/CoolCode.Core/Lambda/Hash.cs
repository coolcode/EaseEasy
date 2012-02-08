using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoolCode {
	/// <summary>
	/// Untyped, case insensitive dictionary that can be initialised using lambdas.
	/// </summary>
	/// <example>
	/// <![CDATA[
	///	IDictionary dict = new Hash(id => "foo", @class => "bar");
	/// ]]>
	/// </example>
	public class Hash : Hash<object> {
		public Hash(params Func<object, object>[] hash)
			: base(hash) {
		}
	}

	/// <summary>Case insensitive, strongly typed dictionary with string keys and <typeparamref name="TValue"/> values that can be initialised using lambdas.</summary>
	/// <typeparam name="TValue">The type of values to create.</typeparam>
	/// <example lang="c#">
	/// <![CDATA[
	/// IDictionary<string,string> dict = new Dictionary<string, string>(2, StringComparer.OrdinalIgnoreCase);
	/// dict.Add("id", "foo");
	/// dict.Add("class", "bar");
	/// IDictionary<string, string> dict = new Hash<string>(id => "foo", @class => "bar");
	/// ]]>
	/// </example>
	public class Hash<TValue> : Dictionary<string, TValue> {
		public Hash(params Func<object, TValue>[] hash)
			: base(hash == null ? 0 : hash.Length, StringComparer.OrdinalIgnoreCase) {
			if (hash != null) {
				foreach (var func in hash) {
					Add(func.Method.GetParameters()[0].Name, func(null));
				}
			}
		}

		/// <summary>Creates an empty case insensitive dictionary of <see cref="string"/> keys and <typeparam name="TValue" /> values.</summary>
		public static Dictionary<string, TValue> Empty {
			get { return new Dictionary<string, TValue>(0, StringComparer.OrdinalIgnoreCase); }
		}
	}
}
