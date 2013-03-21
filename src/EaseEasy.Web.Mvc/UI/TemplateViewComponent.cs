using System.IO;
using System.Web.Script.Serialization;

namespace EaseEasy.Web.Mvc.UI {

	public class TemplateViewComponent<TModel> : ViewComponent where TModel : class {
		public TemplateViewComponent() {
			this.Template = new HtmlTemplate<TModel>();
		}

		[ScriptIgnore]
		public IHtmlTemplate<TModel> Template { get; set; }

		public override void RenderContent(TextWriter writer) {
			if (!Template.IsEmpty)
				Template.WriteTo(writer);
		}
	}

	public class TemplateViewComponent : ViewComponent {
		public TemplateViewComponent() {
			this.Template = new HtmlTemplate();
		}

		[ScriptIgnore]
		public IHtmlTemplate Template { get; set; }

		public override void RenderContent(TextWriter writer) {
			if (!Template.IsEmpty)
				Template.WriteTo(writer);
		}
	}
}
