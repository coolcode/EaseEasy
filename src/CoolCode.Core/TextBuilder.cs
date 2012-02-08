using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoolCode {
	/// <summary>
	/// 使程序保留类似“字符串+=”的方式而又不至于影响性能
	/// <see cref="http://www.cnblogs.com/coolcode/archive/2009/10/13/StringJoiner.html"/>
	/// <example>
	/// <para>TextBuilder s = string.Empty;</para>
	/// <para>s += "Bruce";</para>
	/// <para>s += "Who?";</para>
	/// <para>s += "a guy think sth different...";</para>
	/// </example>
	/// </summary>
	public class TextBuilder {
		protected StringBuilder Builder = new StringBuilder();

		public static TextBuilder operator +(TextBuilder self, string value) {
			self.Builder.Append(value);
			return self;
		}

		public static implicit operator TextBuilder(string value) {
			TextBuilder text = new TextBuilder();
			text.Builder.Append(value);
			return text;
		}

		public static implicit operator string(TextBuilder value) {
			return value.ToString();
		}

		public override string ToString() {
			return this.Builder.ToString();
		}
	}
}
