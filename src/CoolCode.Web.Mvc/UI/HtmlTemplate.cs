using System;
using System.IO;

namespace CoolCode.Web.Mvc.UI {
	public class HtmlTemplate<T> : IHtmlTemplate<T> {
		public T DataItem { get; set; }

		public virtual Action<T> Content { get; set; }

		public Func<T, object> InlineContent { get; set; }

		public virtual void WriteTo(T dataItem, TextWriter writer) {
			this.DataItem = dataItem;
			this.WriteTo(writer);
		}

		public virtual void WriteTo(TextWriter writer) {
			if (Content != null) {
				Content.Invoke(DataItem);
			}
			else {
				if (InlineContent != null)
					writer.Write(InlineContent(DataItem));
			}
		}

		public virtual bool IsEmpty {
			get {
				return ((Content == null) && (InlineContent == null));
			}
		}
	}

	public class HtmlTemplate : IHtmlTemplate {
		public Action Content { get; set; }

		public Func<dynamic, object> InlineContent { get; set; }

		public void WriteTo(TextWriter writer) {
			if (Content != null) {
				Content.Invoke();
			}
			else {
				if (InlineContent != null)
					writer.Write(InlineContent(null));
			}
		}

		public bool IsEmpty {
			get {
				return ((Content == null) && (InlineContent == null));
			}
		}

	}
}
