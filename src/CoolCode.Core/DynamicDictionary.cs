using System.Collections.Generic;
using System.Dynamic;

namespace CoolCode {

	public static class DynamicDictionaryExtesion {
		public static DynamicDictionary ToDynamicDictionary(this IDictionary<string, object> dictionary) {
			return new DynamicDictionary(dictionary);
		}
	}

	public class DynamicDictionary : DynamicObject {
		private IDictionary<string, object> dictionary;

		public DynamicDictionary() {
			this.dictionary = new Dictionary<string, object>();
		}

		public DynamicDictionary(IDictionary<string, object> dictionary) {
			this.dictionary = dictionary;
		}

		public object this[string name] {
			get {
				object result;
				dictionary.TryGetValue(name, out result);
				return result;
			}
		}

		public int Count {
			get {
				return dictionary.Count;
			}
		}

		public override bool TryGetMember(GetMemberBinder binder, out object result) {
			dictionary.TryGetValue(binder.Name, out result);
			return true;
		}

		public override bool TrySetMember(SetMemberBinder binder, object value) {
			dictionary[binder.Name] = value;
			return true;
		}
	}
}
