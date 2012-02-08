using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace CoolCode {
	public static class StringExtensions {
		#region 常用

		/// <summary>
		/// 字符串转化成其他指定类型
		/// </summary>
		/// <typeparam name="T">指定类型</typeparam>
		/// <param name="value"></param>
		/// <param name="throwException">当调用该方法出错时是否抛异常</param>
		/// <returns></returns>
		public static T To<T>(this string value, bool throwException = false) {
			try {
				T t = (T)CoolCode.Reflection.TypeParser.Parse(typeof(T), value);
				return t;
			}
			catch {
				if (throwException) {
					throw;
				}
				return default(T);
			}
		}

		/// <summary>
		/// 是否英文字母
		/// </summary>
		/// <param name="c"></param>
		/// <returns></returns>
		public static bool IsLetter(this char c) {
			return (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z');
		}

		/// <summary>
		/// 获取下一个字符，如A下一个字符是B
		/// </summary>
		/// <param name="c"></param>
		/// <returns></returns>
		public static char Next(this char c) {
			int u = Convert.ToInt32(c);
			return Convert.ToChar(u + 1);
		}

		/// <summary>
		/// 获取字符串中的英文字母，其他非英文字母的字符将被去除
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		public static string GetLetters(this string s) {
			StringBuilder sb = new StringBuilder();
			foreach (char c in s) {
				if (IsLetter(c)) {
					sb.Append(c);
				}
			}
			return sb.ToString();
		}

		/// <summary>
		/// 验证是否为数字
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static bool IsNumeric(this string value) {
			if (string.IsNullOrEmpty(value))
				return false;
			return Regex.IsMatch(value, @"^[0-9]*$");
		}

		/// <summary>
		/// 检测是否符合email格式
		/// </summary>
		/// <param name="strEmail">要判断的email字符串</param>
		/// <returns>判断结果</returns>
		public static bool IsEmail(this string strEmail) {
			return Regex.IsMatch(strEmail, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
		}

		/// <summary>
		/// 检测是否是正确的Url
		/// </summary>
		/// <param name="strUrl">要验证的Url</param>
		/// <returns>判断结果</returns>
		public static bool IsURL(this string strUrl) {
			return Regex.IsMatch(strUrl, @"^(http|https)\://([a-zA-Z0-9\.\-]+(\:[a-zA-Z0-9\.&%\$\-]+)*@)*((25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9])\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[0-9])|localhost|([a-zA-Z0-9\-]+\.)*[a-zA-Z0-9\-]+\.(com|edu|gov|int|mil|net|org|biz|arpa|info|name|pro|aero|coop|museum|[a-zA-Z]{1,10}))(\:[0-9]+)*(/($|[a-zA-Z0-9\.\,\?\'\\\+&%\$#\=~_\-]+))*$");
		}

		public static string GetEmailHostName(this string strEmail) {
			if (strEmail.IndexOf("@") < 0) {
				return string.Empty;
			}
			return strEmail.Substring(strEmail.LastIndexOf("@")).ToLower();
		}

		/// <summary>
		/// 是否时间格式字符串hh:mm:ss
		/// </summary>
		/// <returns></returns>
		public static bool IsTime(this string timeval) {
			return Regex.IsMatch(timeval, @"^((([0-1]?[0-9])|(2[0-3])):([0-5]?[0-9])(:[0-5]?[0-9])?)$");
		}

		/// <summary>
		/// 判断字符串是否符合yyyy-MM-dd的日期格式
		/// </summary>
		/// <param name="value">待判断字符串</param>
		/// <returns>判断结果</returns>
		public static bool IsDate(string value) {
			return Regex.IsMatch(value, @"(\d{4})-(\d{1,2})-(\d{1,2})");
		}

		public static string ToSystemDateFormat(this string date) {
			var dateFormat = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat;
			string dateMask = dateFormat.ShortDatePattern;
			DateTime dateTime;
			if (DateTime.TryParse(date, out dateTime)) {
				return dateTime.ToString(dateMask, System.Globalization.CultureInfo.InvariantCulture);
			}
			return date;
		}

		/// <summary>
		/// 返回字符串真实长度, 1个汉字长度为2
		/// </summary>
		/// <returns>字符长度</returns>
		public static int GetStringLength(this string value) {
			return Encoding.Default.GetBytes(value).Length;
		}

		/// <summary>
		/// 获取文件扩展名
		/// </summary>
		/// <param name="filename"></param>
		/// <returns></returns>
		public static string GetFileExtName(this string filename) {
			string[] array = filename.Trim().Split('.');
			Array.Reverse(array);
			return array[0];
		}

		public static bool IsRelativePath(this string path) {
			if (path.Contains(@":\") || path.Contains(@"\\"))/*Windows path*/ {
				return false;
			}
			if (path.Contains(@"://"))/*url*/ {
				return false;
			}

			return path.All(c => (c.IsLetter() || c == '\\') || c == '/');
		}

		public static string GetParentPath(this string path, char separator = '\\') {
			int ind = path.LastIndexOf(separator);
			if (ind > 0) {
				return path.Substring(0, ind);
			}

			return path;
		}

		/// <summary>
		/// 截断
		/// </summary>
		/// <param name="original"></param>
		/// <param name="maxLength">截取长度，超过长度的字符以省略号（...）显示，该参数必须大于3</param>
		/// <remarks>该方法不会对参数进行有效性检查</remarks>
		/// <returns></returns>
		public static string Cut(this string original, int maxLength) {
			return (original != null && original.Length > maxLength) ? original.Substring(0, maxLength - 3) + "..." : original;
		}


		#endregion

		#region 不常用

		public static string Repeat(this string value, int n) {
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < n; i++)
				sb.Append(value);
			return sb.ToString();
		}

		public static string Repeat(this char c, int n) {
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < n; i++)
				sb.Append(c);
			return sb.ToString();
		}

		public static string Join(this string[] value, string separator) {
			return string.Join(separator, value);
		}

		#region Web

		/// <summary>
		/// 从Html中获取标题
		/// </summary>
		/// <param name="html"></param>
		/// <returns></returns>
		public static string GetHtmlTitle(this string html) {
			string title = string.Empty;
			Regex reg = new Regex(@"(?m)<title[^>]*>(?<title>(?:\w|\W)*?)</title[^>]*>", RegexOptions.Multiline | RegexOptions.IgnoreCase);
			Match mc = reg.Match(html);
			if (mc.Success)
				title = mc.Groups["title"].Value.Trim();
			return title;
		}

		/// <summary>
		/// 从HTML中获取文本
		/// </summary>
		/// <param name="html"></param>
		/// <returns></returns>
		public static string HtmlToText(this string html) {
			Regex regEx = new Regex(@"</?[^>]*>", RegexOptions.IgnoreCase);
			//string regEx_script = @"<[\s]*?script[^>]*?>[\s\S]*?<[\s]*?\/[\s]*?script[\s]*?>"; // 定义script的正则表达式{或<script[^>]*?>[\s\S]*?<\/script>
			string result = regEx.Replace(html, string.Empty);
			result = result.Replace("&nbsp;", string.Empty)//移除空格
				.Replace("\n", string.Empty)
				.Replace("\r", string.Empty)
				.Trim();

			return result;
		}

		/// <summary>
		/// 从HTML中获取文本 ,保留br,p,img
		/// </summary>
		/// <param name="html"></param>
		/// <returns></returns>
		public static string HtmlFilter(this string html) {
			Regex regEx = new Regex(@"</?(?!br|/?p|img)[^>]*>", RegexOptions.IgnoreCase);

			return regEx.Replace(html, "");
		}

		/// <summary>
		/// 是否为ip
		/// </summary>
		/// <param name="ip"></param>
		/// <returns></returns>
		public static bool IsIP(this string ip) {
			return Regex.IsMatch(ip, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$");
		}

		public static bool IsIPSect(this string ip) {
			return Regex.IsMatch(ip, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){2}((2[0-4]\d|25[0-5]|[01]?\d\d?|\*)\.)(2[0-4]\d|25[0-5]|[01]?\d\d?|\*)$");
		}

		/// <summary>
		/// 过滤HTML中的不安全标签
		/// </summary>
		/// <param name="content"></param>
		/// <returns></returns>
		public static string HtmlFilterUnsafe(this string content) {
			content = Regex.Replace(content, @"(\<|\s+)o([a-z]+\s?=)", "$1$2", RegexOptions.IgnoreCase);
			content = Regex.Replace(content, @"(script|frame|form|meta|behavior|style)([\s|:|>])+", "$1.$2", RegexOptions.IgnoreCase);
			return content;
		}

		private static Regex RegexBr = new Regex(@"(\r\n)", RegexOptions.IgnoreCase);
		private static Regex RegexFont = new Regex(@"<font color=" + "\".*?\"" + @">([\s\S]+?)</font>", GetRegexCompiledOptions());

		/// <summary>
		/// 得到正则编译参数设置
		/// </summary>
		/// <returns>参数设置</returns>
		private static RegexOptions GetRegexCompiledOptions() {
			return RegexOptions.None;
		}

		/// <summary>
		/// 清除给定字符串中的回车及换行符
		/// </summary>
		/// <param name="value">要清除的字符串</param>
		/// <returns>清除后返回的字符串</returns>
		public static string ClearBR(this string value) {
			for (Match m = RegexBr.Match(value); m.Success; m = m.NextMatch()) {
				value = value.Replace(m.Groups[0].ToString(), "");
			}

			return value;
		}

		/// <summary>
		/// 去掉font标签
		/// </summary>
		/// <param name="content"></param>
		/// <returns></returns>
		public static string HtmlFilterFontTag(this string content) {
			Match m = RegexFont.Match(content);
			if (m.Success) {
				return m.Groups[1].Value;
			}
			return content;
		}

		#endregion

		#endregion
	}
}
