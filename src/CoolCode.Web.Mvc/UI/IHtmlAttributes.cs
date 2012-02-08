using System;
using System.Collections;
using System.Collections.Generic;

namespace CoolCode.Web.Mvc.UI {
	public interface IHtmlAttributes : IDictionary<string, string>, IDictionary {
		new int Count { get; }
		new IEnumerator<KeyValuePair<string, string>> GetEnumerator();
		int GetEstLength();
	}

	public static class HtmlAttributeExtensions {
		public static void AddStyleIfNotExists(this IHtmlAttributes attributes, string name, string value) {
			string newStyle = string.Format("{0}:{1};", name, value);
			if (!attributes.ContainsKey("style") || attributes["style"] == null) {
				attributes["style"] = newStyle;
			}
			else {
				string curStyle = attributes["style"];
				if (!newStyle.Contains("width")) {
					attributes["style"] = newStyle + curStyle;
				}
			}
		}

		public static void CopyTo(this IHtmlAttributes attributesFrom, IDictionary<string, string> attributesTo) {
			attributesFrom.ForEach(c => attributesTo[c.Key] = Convert.ToString(c.Value));
		}

	}
}
