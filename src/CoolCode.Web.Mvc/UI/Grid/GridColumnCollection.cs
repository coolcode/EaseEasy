using System.Collections;
using System.Collections.Generic;

namespace CoolCode.Web.Mvc.UI {
	public class GridColumnCollection<T> : ViewComponent, ICollection<GridColumn<T>> {
		private readonly List<GridColumn<T>> _columns = new List<GridColumn<T>>();

		public IEnumerator<GridColumn<T>> GetEnumerator() {
			return _columns.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}

		public void Add(GridColumn<T> column) {
			_columns.Add(column);
		}

		void ICollection<GridColumn<T>>.Clear() {
			_columns.Clear();
		}

		bool ICollection<GridColumn<T>>.Contains(GridColumn<T> column) {
			return _columns.Contains(column);
		}

		void ICollection<GridColumn<T>>.CopyTo(GridColumn<T>[] array, int arrayIndex) {
			_columns.CopyTo(array, arrayIndex);
		}

		bool ICollection<GridColumn<T>>.Remove(GridColumn<T> column) {
			return _columns.Remove(column);
		}

		int ICollection<GridColumn<T>>.Count {
			get { return _columns.Count; }
		}

		bool ICollection<GridColumn<T>>.IsReadOnly {
			get { return false; }
		}
	}

}
