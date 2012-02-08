using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Data.Common;
using System.Reflection;

namespace CoolCode.Data {
	public class ObjectReader<T> : IEnumerable<T>, IEnumerable where T : class, new() {
		private Enumerator enumerator;
		public ObjectReader(DbDataReader reader) {
			this.enumerator = new Enumerator(reader);
		}

		public IEnumerator<T> GetEnumerator() {
			Enumerator e = this.enumerator;
			if (e == null) {
				throw new InvalidOperationException("Cannot enumerate more than once");
			}
			this.enumerator = null;
			return e;
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return this.GetEnumerator();
		}

		class Enumerator : IEnumerator<T>, IEnumerator, IDisposable {
			private DbDataReader reader;
			private PropertyInfo[] fields;
			private int[] fieldLookup;
			private T current;

			internal Enumerator(DbDataReader reader) {
				this.reader = reader;
				this.fields = typeof(T).GetProperties();
			}

			public T Current {
				get { return this.current; }
			}

			object IEnumerator.Current {
				get { return this.current; }
			}

			public bool MoveNext() {
				if (this.reader.Read()) {
					if (this.fieldLookup == null) {
						this.InitFieldLookup();
					}
					T instance = new T();
					for (int i = 0; i < this.fields.Length; i++) {
						int index = this.fieldLookup[i];
						if (index >= 0) {
							var fi = this.fields[i];
							if (this.reader.IsDBNull(index)) {
								fi.SetValue(instance, null, null);
							}
							else {
								fi.SetValue(instance, this.reader.GetValue(index), null);
							}
						}
					}
					this.current = instance;
					return true;
				}
				return false;
			}

			public void Reset() {

			}

			public void Dispose() {
				this.reader.Dispose();
			}

			private void InitFieldLookup() {
				var map = new Dictionary<string, int>(StringComparer.InvariantCultureIgnoreCase);
				for (int i = 0; i < this.reader.FieldCount; i++) {
					map.Add(this.reader.GetName(i), i);
				}
				this.fieldLookup = new int[this.fields.Length];
				for (int i = 0; i < this.fields.Length; i++) {
					int index;
					if (map.TryGetValue(this.fields[i].Name, out index)) {
						this.fieldLookup[i] = index;
					}
					else {
						this.fieldLookup[i] = -1;
					}
				}
			}
		}

	}
}
