using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace CoolCode.Web.Mvc.UI {
	public class SearchView : TemplateViewComponent { 
		public string Target { get; set; } 

		public override void RenderContent(TextWriter writer) {
			var builder = new TagBuilder("table");
			builder.MergeAttribute("width", "100%");
			builder.AddCssClass("search");

			var trBuilder = new TagBuilder("tr");
			var tdSearchConditionBuilder = new TagBuilder("td");
			tdSearchConditionBuilder.MergeAttribute("width", "100%");

			using (var stringWriter = new StringWriter(new StringBuilder())) {
				base.RenderContent(stringWriter);
				tdSearchConditionBuilder.InnerHtml = stringWriter.ToString();
			}  

			trBuilder.InnerHtml = tdSearchConditionBuilder.ToString(TagRenderMode.Normal);

			var tdSearchButtonBuilder = new TagBuilder("td");
			tdSearchButtonBuilder.MergeAttribute("width", "100px");
			tdSearchButtonBuilder.InnerHtml =
				string.Format("<input type=\"button\" rel=\"search\" target=\"{0}\" value=\"查询\" /> ", this.Target);

			trBuilder.InnerHtml += tdSearchButtonBuilder.ToString(TagRenderMode.Normal);

			builder.InnerHtml = trBuilder.ToString(TagRenderMode.Normal);

			writer.Write(builder.ToString(TagRenderMode.Normal));
		}

	}
}
