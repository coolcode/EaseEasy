using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace EaseEasy {
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	public class AttachStringAttribute : Attribute {
		public string Value { get; private set; }

		public AttachStringAttribute(string value) {
			this.Value = value;
		}
	}

	public static class EnumAttachStringExtensions {
		private readonly static Dictionary<string, string> _cacheAttachedData = new Dictionary<string, string>();
		private readonly static object lockObject = new object();

		/// <summary>
		/// 获取Enum中标记[AttachString]的字符串
		/// </summary>
		/// <param name="en"></param>
		/// <returns></returns>
		public static string GetAttachedString<T>(this T en) {
			string value;
			string enumFullName = GetEnumFullName(en);
			if (!_cacheAttachedData.TryGetValue(enumFullName, out value)) {
				lock (lockObject) {
					if (!_cacheAttachedData.TryGetValue(enumFullName, out value)) {
						value = GetAttachedStringHelper(en);
						_cacheAttachedData.Add(enumFullName, value);
					}
				}
			}
			return value;
		}

		private static string GetAttachedStringFromAttribute(this ICustomAttributeProvider provider) {
			var attributes = (AttachStringAttribute[])provider.GetCustomAttributes(typeof(AttachStringAttribute), false);

			if (attributes == null || attributes.Length == 0) {
				throw new NotSupportedException("e.g. [AttachString(\"foo\")]");
			}

			return attributes.First().Value;
		}

		private static string GetAttachedStringHelper<T>(this T value) {
			return value.GetType().GetField(value.ToString()).GetAttachedStringFromAttribute();
		}

		private static string GetEnumFullName<T>(T en) {
			return en.GetType().FullName + "." + en.ToString();
		}
	}

}
