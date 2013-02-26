using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Web.Mvc;
using System.Web.UI;

namespace CoolCode.Web.Mvc.UI {
	public abstract class ViewComponent {
		private string _id;
		private HtmlHelper _htmlHelper;

		public IViewDataContainer ViewDataContainer { get; internal set; }

		public ViewContext ViewContext { get; internal set; }

		public ViewDataDictionary ViewData { get { return ViewContext.ViewData; } }

		public string Name { get; set; }

		public int Width { get; set; }

		public int Height { get; set; }

		public virtual string TagName { get { return "div"; } }

		public IHtmlAttributes HtmlAttributes { get; private set; }

		protected internal HtmlHelper Html {
			get {
				if (_htmlHelper == null)
					_htmlHelper = new HtmlHelper(this.ViewContext, this.ViewDataContainer);
				return _htmlHelper;
			}
		}

		public ViewComponent() {
			this.HtmlAttributes = new HtmlAttributes();
		}

		public virtual void Render(TextWriter writer) {
			RenderBeginTag(writer);

			RenderContent(writer);

			RenderEndTag(writer);
		}

		public virtual void RenderContent(TextWriter writer) { }

		public string Id {
			get {
				if ((string.IsNullOrEmpty(_id)) && (!string.IsNullOrEmpty(Name))) {
					var tag = new TagBuilder(TagName);
					tag.GenerateId(Name);
					if (tag.Attributes.ContainsKey("id"))
						_id = tag.Attributes["id"];
					else
						_id = Name;
				}
				return _id;
			}
			private set { _id = value; }
		}

		public void MergeAttribute(string key, string value) {
			this.MergeAttribute(key, value, false);
		}

		public void MergeAttribute(string key, string value, bool replaceExisting) {
			if (string.IsNullOrEmpty(key)) {
				throw new ArgumentNullException("key");
			}

			if (replaceExisting || !this.HtmlAttributes.ContainsKey(key)) {
				this.HtmlAttributes[key] = value;
			}
            else if (this.HtmlAttributes.ContainsKey(key))
            {
                this.HtmlAttributes[key] += " " + value;
            }
		}

		public void MergeAttributes<TKey, TValue>(IDictionary<TKey, TValue> attributes) {
			this.MergeAttributes(attributes, false);
		}

		public void MergeAttributes<TKey, TValue>(IDictionary<TKey, TValue> attributes, bool replaceExisting) {
			if (attributes != null) {
				foreach (KeyValuePair<TKey, TValue> pair in attributes) {
					string key = Convert.ToString(pair.Key, CultureInfo.InvariantCulture);
					string value = Convert.ToString(pair.Value, CultureInfo.InvariantCulture);
					this.MergeAttribute(key, value, replaceExisting);
				}
			}
		}

		public virtual void RenderBeginTag(TextWriter writer) {
			TagBuilder tag = new TagBuilder(TagName);

			if (!string.IsNullOrEmpty(Name)) {
				tag.GenerateId(Name);
				if (!tag.Attributes.ContainsKey("id"))
					tag.MergeAttribute("id", Name);

				Id = tag.Attributes["id"];

				if (TagName.Equals("input", StringComparison.OrdinalIgnoreCase) || TagName.Equals("textarea", StringComparison.OrdinalIgnoreCase))
					tag.MergeAttribute("name", Name);
			}

			if (Width > 0 || Height > 0) {
				string style = (Width > 0 ? "width:" + Width.ToString() + "px;" : "") + (Height > 0 ? "height:" + Height.ToString() + "px;" : "");
				MergeAttribute("style", style);
			}

			if (this.HtmlAttributes != null) {
				tag.MergeAttributes(HtmlAttributes);
			}

			writer.Write(tag.ToString(TagRenderMode.StartTag));
		}

		public virtual void RenderEndTag(TextWriter writer) {
			TagBuilder tag = new TagBuilder(TagName);
			writer.Write(tag.ToString(TagRenderMode.EndTag));
		}

		public string ToHtmlString() {
			var result = new StringBuilder();
			using (var writer = new HtmlTextWriter(new StringWriter(result))) {
				Render(writer);
			}

			return result.ToString();
		}
	}

}
