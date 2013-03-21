using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EaseEasy {
	public class ValueText {
		public string Value { get; set; }
		public string Text { get; set; }
		public bool Selected { get; set; }

		public ValueText() {
		}

		public ValueText(string value, string text, bool selected = false) {
			this.Value = value;
			this.Text = text;
			this.Selected = selected;
		}
	}

	public static class ValueTextExtensions {
		public static IEnumerable<ValueText> SelectValues(this IEnumerable<ValueText> source, string selectedValues) {
			if (string.IsNullOrEmpty(selectedValues)) {
				return source;
			}
			return source.SelectValues(selectedValues.Split(new char[]{','} , StringSplitOptions.RemoveEmptyEntries));
		}

		public static IEnumerable<ValueText> SelectValues(this IEnumerable<ValueText> source, string[] selectedValues) {
			foreach (ValueText item in source) {
				if (selectedValues.Contains(item.Value)) {
					item.Selected = true;
				}
			}
			return source;
		}
	}
}
