using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace CoolCode.Web.Mvc.UI {
	/// <summary>
	/// äÖÈ¾GridViewµÄ»ùÀà
	/// </summary>
	public abstract class BaseGridViewRenderer<T> : IGridViewRenderer<T> {
		public ViewContext ViewContext { get; private set; }
		public IViewDataContainer ViewDataContainer { get; private set; }

		public IGridView<T> GridModel { get; private set; }
		public TextWriter Writer { get; private set; }

		protected IEnumerable<T> DataSource { get; set; }
		protected DomBuilder Dom = new DomBuilder();

		public BaseGridViewRenderer(ViewContext context, IViewDataContainer container) {
			ViewContext = context;
			ViewDataContainer = container;
		}

		public void Render(IGridView<T> gridModel, TextWriter output) {
			Writer = output;
			GridModel = gridModel;
			DataSource = gridModel.DataSource;

			RenderGridStart();

			if (IsDataSourceEmpty()) {
				RenderEmpty();
			}
			else {
				RenderTable();
			}

			RenderGridEnd();
		}

		protected void RenderText(string text) {
			Dom.Write(text);
		}

		protected virtual void RenderTable() {
			RenderTableStart();
			RenderHeader();
			RenderItems();
			RenderTableEnd();
		}

		protected virtual void RenderItems() {
			RenderBodyStart();

			int rowIndex = 0;
			var columnNames = GridModel.Columns.Select(c => c.Name);
			foreach (var item in DataSource) {
				RenderItem(new GridRow<T>(item, rowIndex, columnNames));
				rowIndex++;
			}

			RenderBodyEnd();
		}

		protected virtual void RenderItem(GridRow<T> rowData) {
			RenderRowStart(rowData);

			foreach (var column in Columns()) {
				RenderStartCell(column, rowData);

				RenderCellValue(column, rowData);

				RenderEndCell();
			}

			RenderRowEnd();
		}

		protected virtual void RenderCellValue(GridColumn<T> column, GridRow<T> rowData) {
			var cellValue = column.GetValue(rowData);

			if (cellValue != null) {
				RenderText(cellValue.ToString());
			}
		}

		protected virtual bool RenderHeader() {
			RenderHeadStart();

			foreach (var column in Columns()) {
				RenderHeaderCellStart(column);
				RenderHeaderText(column);
				RenderHeaderCellEnd();
			}

			RenderHeadEnd();

			return true;
		}

		protected virtual void RenderHeaderText(GridColumn<T> column) {
			RenderText(column.GetHeader());
		}

		protected virtual bool ShouldRenderHeader() {
			return !IsDataSourceEmpty();
		}

		protected bool IsDataSourceEmpty() {
			return DataSource == null || !DataSource.Any();
		}

		protected IEnumerable<GridColumn<T>> VisibleColumns() {
			return GridModel.Columns.Where(x => x.Visible);
		}

		protected IEnumerable<GridColumn<T>> Columns() {
			return GridModel.Columns;
		}

		protected abstract void RenderTableStart();
		protected abstract void RenderTableEnd();
		protected abstract void RenderHeaderCellStart(GridColumn<T> column);
		protected abstract void RenderHeaderCellEnd();
		protected abstract void RenderRowStart(GridRow<T> rowData);
		protected abstract void RenderRowEnd();
		protected abstract void RenderEndCell();
		protected abstract void RenderStartCell(GridColumn<T> column, GridRow<T> rowViewData);
		protected abstract void RenderHeadStart();
		protected abstract void RenderHeadEnd();
		protected abstract void RenderGridStart();
		protected abstract void RenderGridEnd();
		protected abstract void RenderEmpty();
		protected abstract void RenderBodyStart();
		protected abstract void RenderBodyEnd();
	}
}
