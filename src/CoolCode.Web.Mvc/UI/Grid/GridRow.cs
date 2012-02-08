using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace CoolCode.Web.Mvc.UI {
	public class GridRow<T> : GridRow {
		/// <summary>
		/// The current item for this row in the data source.
		/// </summary>
		public new T Value { get { return base.Value; } }

		public GridRow(T item, int index, IEnumerable<string> columnNames)
			: base(item, index, columnNames) {
		}
	}

	public class GridRow : DynamicObject, IEnumerable<object> {
		private const string RowIndexMemberName = "ROW";
		private const BindingFlags BindFlags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.IgnoreCase;

		private IEnumerable<string> _columnNames;
		private IDynamicMetaObjectProvider _dynamic;
		private int _rowIndex;
		private object _value;
		private IEnumerable<dynamic> _values;

		public GridRow(object value, int rowIndex, IEnumerable<string> columnNames) {
			_columnNames = columnNames;
			_value = value;
			_rowIndex = rowIndex;
			_dynamic = value as IDynamicMetaObjectProvider;
		}

		public object this[int index] {
			get {
				if ((index < 0) || (index >= _columnNames.Count())) {
					throw new ArgumentOutOfRangeException("index");
				}
				return this.Skip(index).First();
			}
		}

		public object this[string name] {
			get {
				if (String.IsNullOrEmpty(name)) {
					throw new ArgumentException("Argument cannot be null or empty", "name");
				}
				object value = null;
				if (!TryGetMember(name, out value)) {
					throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture,
						 "Column {0} not found!", name));
				}
				return value;
			}
		}

		public dynamic Value {
			get {
				return _value;
			}
		}

		public int RowIndex {
			get {
				return _rowIndex;
			}
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}

		public IEnumerator<object> GetEnumerator() {
			if (_values == null) {
				_values = _columnNames.Select(c => this[c]);
			}
			return _values.GetEnumerator();
		}

		public override bool TryGetMember(GetMemberBinder binder, out object result) {
			// Try to get the row index
			if (TryGetRowIndex(binder.Name, out result)) {
				return true;
			}

			// Try to evaluate the dynamic member based on the binder
			if (_dynamic != null && DynamicHelper.TryGetMemberValue(_dynamic, binder, out result)) {
				return true;
			}

			return TryGetComplexMember(_value, binder.Name, out result);
		}

		internal bool TryGetMember(string memberName, out object result) {

			// Try to get the row index
			if (TryGetRowIndex(memberName, out result)) {
				return true;
			}

			// Try to evaluate the dynamic member based on the name
			if (_dynamic != null && DynamicHelper.TryGetMemberValue(_dynamic, memberName, out result)) {
				return true;
			}

			// Support '.' for navigation properties
			return TryGetComplexMember(_value, memberName, out result);
		}

		public override string ToString() {
			return _value.ToString();
		}

		private bool TryGetRowIndex(string memberName, out object result) {
			result = null;
			if (String.IsNullOrEmpty(memberName)) {
				return false;
			}

			if (memberName == RowIndexMemberName) {
				result = _rowIndex;
				return true;
			}

			return false;
		}

		private static bool TryGetComplexMember(object obj, string name, out object result) {
			result = null;

			string[] names = name.Split('.');
			for (int i = 0; i < names.Length; i++) {
				if ((obj == null) || !TryGetMember(obj, names[i], out result)) {
					result = null;
					return false;
				}
				obj = result;
			}
			return true;
		}

		private static bool TryGetMember(object obj, string name, out object result) {
			PropertyInfo property = obj.GetType().GetProperty(name, BindFlags);
			if ((property != null) && (property.GetIndexParameters().Length == 0)) {
				result = property.GetValue(obj, null);
				return true;
			}
			result = null;
			return false;
		}
	}
}
