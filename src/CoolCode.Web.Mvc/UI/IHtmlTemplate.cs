using System;
using System.IO;

namespace CoolCode.Web.Mvc.UI {
	 
	public interface IHtmlTemplate<T> {
		Action<T> Content { get; set; }

		Func<T, object> InlineContent { get; set; }

		bool IsEmpty { get; }

		void WriteTo(TextWriter writer);
	}
	 
	public interface IHtmlTemplate {
		Action Content { get; set; }

		Func<dynamic, object> InlineContent { get; set; }

		bool IsEmpty { get; }

		void WriteTo(TextWriter writer);
	}

}
