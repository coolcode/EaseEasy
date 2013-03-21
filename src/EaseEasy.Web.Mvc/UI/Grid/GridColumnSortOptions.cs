using System.Data.SqlClient;

namespace EaseEasy.Web.Mvc.UI {
	public class GridColumnSortOptions {
		public SortOrder SortOrder { get; set; }

		public string SortByQueryParameterName { get; set; }

		public string SortOrderQueryParameterName { get; set; }
	}
}