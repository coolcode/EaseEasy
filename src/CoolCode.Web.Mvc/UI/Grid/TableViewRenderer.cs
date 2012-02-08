using System.Web.Mvc;

namespace CoolCode.Web.Mvc.UI {
	/// <summary>
	/// HTML table Grid
	/// </summary>
	public class TableViewRenderer<T> : BaseGridViewRenderer<T> {
		private const string DefaultCssClass = "bbit-grid";

		public TableViewRenderer(ViewContext context, IViewDataContainer container)
			: base(context, container) {
		}

		protected override void RenderGridStart() {
			TagBuilder tagBuilder = Dom.CreateTag("div");
			tagBuilder.AddCssClass(DefaultCssClass);
			tagBuilder.GenerateId(GridModel.Id + "_inner");
		}

		protected override void RenderGridEnd() {
			string html = Dom.ToString();
			Writer.Write(html);
		}

		protected override void RenderEmpty() {
			RenderTableStart();
			RenderHeader();
			RenderBodyStart();

			TagBuilder tr = Dom.CreateTag("tr");
			tr.AddCssClass("erow empty-text");

			TagBuilder td = Dom.CreateTag("td");
			td.Attributes["colspan"] = "50";

			TagBuilder div = Dom.CreateTag("div");
			div.InnerHtml = GridModel.EmptyText;

			Dom.EndTag();
			Dom.EndTag();
			Dom.EndTag();

			RenderBodyEnd();
			RenderTableEnd();
		}


		protected override void RenderTableStart() {
			TagBuilder tagBuilder = Dom.CreateTag("table");
			tagBuilder.Attributes["border"] = "0";
			tagBuilder.Attributes["cellpadding"] = "0";
			tagBuilder.Attributes["cellspacing"] = "0";
			GridModel.Attributes.CopyTo(tagBuilder.Attributes);
		}

		protected override void RenderTableEnd() {
			Dom.EndTag();
		}

		protected override void RenderHeaderCellStart(GridColumn<T> column) {
			if (column.Sortable && column.Name == ViewContext.Controller.ValueOf<string>(GridModel.SortFieldName)) {
				string sortOrderCss = column.SortOptions.SortOrder == System.Data.SqlClient.SortOrder.Ascending ? "desc" : "asc";
				column.HeaderAttributes.AddIfNotExists("class", sortOrderCss);
			}

			IHtmlAttributes attributes = new HtmlAttributes();
			if (column.Width > 0) {
				attributes.AddStyleIfNotExists("width", column.Width + "px");
			}

			if (!column.Visible) {
				attributes.AddStyleIfNotExists("display", "none");
			}

			TagBuilder th = Dom.CreateTag("th");
			attributes.CopyTo(th.Attributes);

			column.HeaderAttributes.CopyTo(th.Attributes);
		}

		protected override void RenderHeaderCellEnd() {
			Dom.EndTag();
		}

		protected override void RenderRowStart(GridRow<T> rowData) {
			var attributes = new HtmlAttributes();
			attributes.AddIfNotExists("class", () => rowData.RowIndex % 2 == 0 ? "row" : "erow");
			TagBuilder tagBuilder = Dom.CreateTag("tr");
			attributes.CopyTo(tagBuilder.Attributes);
		}

		protected override void RenderRowEnd() {
			Dom.EndTag();
		}

		protected override void RenderStartCell(GridColumn<T> column, GridRow<T> rowData) {
			var attributes = column.HtmlAttributes; //column.Attributes(rowData);

			if (column.Width > 0) {
				attributes.AddStyleIfNotExists("width", column.Width + "px");
			}

			if (attributes.ContainsKey("align")) {
				attributes.AddStyleIfNotExists("text-align", attributes["align"]);
			}

			TagBuilder td = Dom.CreateTag("td");
			if (!column.Visible) {
				var tdAttributes = new HtmlAttributes();
				tdAttributes.AddStyleIfNotExists("display", "none");
				tdAttributes.CopyTo(td.Attributes);
			}

			attributes.CopyTo(td.Attributes);
		}

		protected override void RenderEndCell() {
			Dom.EndTag();
		}

		protected override void RenderHeadStart() {
			TagBuilder thead = Dom.CreateTag("thead");
			TagBuilder tr = Dom.CreateTag("tr");
		}

		protected override void RenderHeadEnd() {
			Dom.EndTag();
			Dom.EndTag();
		}

		protected override void RenderBodyStart() {
			Dom.CreateTag("tbody");
		}

		protected override void RenderBodyEnd() {
			Dom.EndTag();
		}

	}
}