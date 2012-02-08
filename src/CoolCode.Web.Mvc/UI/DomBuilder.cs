using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;

namespace CoolCode.Web.Mvc.UI {
	/// <summary>
	/// 提供快速构建Html的API
	/// </summary>
	public class DomBuilder : StringWriter {
		private Stack<TagBuilder> _tags = new Stack<TagBuilder>();
		//TODO:应该用StringBuilder替换？
		private string _innerHtml = string.Empty;

		public DomBuilder() {
		}

		public TagBuilder CreateTag(string tagName) {
			TagBuilder tagBuilder = new TagBuilder(tagName);
			_tags.Push(tagBuilder);

			return tagBuilder;
		}

		public override void Write(string value) {
			if (_tags.Count == 0) {
				_innerHtml += WrapChildrenHtml(value);
			}
			TagBuilder tagBuilder = _tags.Peek();
			tagBuilder.InnerHtml += WrapChildrenHtml(value);
		}

		public void EndTag() {
			if (_tags.Count == 0)
				throw new Exception("Begin Tag Not Found!");

			TagBuilder tagBuilder = _tags.Pop();
			_innerHtml = tagBuilder.ToString();

			if (_tags.Count == 0)
				return;

			_tags.Peek().InnerHtml += WrapChildrenHtml(_innerHtml);
		}

		protected virtual string WrapChildrenHtml(string html) {
			return html;
			//StringBuilder sb = new StringBuilder();
			//sb.Append("\r\n");
			//sb.Append("\t".Repeat(_tags.Count + 1));
			//sb.Append(html);
			//sb.Append("\r\n");
			//return sb.ToString();
		}

		public override string ToString() {
			Stack<TagBuilder> copyTags = new Stack<TagBuilder>(_tags);
			while (copyTags.Count > 0) {
				TagBuilder tagBuilder = copyTags.Pop();

				if (copyTags.Count == 0) {
					return tagBuilder.ToString();
				}

				copyTags.Peek().InnerHtml += tagBuilder.ToString();
			}

			return _innerHtml;
		}
	}
}
