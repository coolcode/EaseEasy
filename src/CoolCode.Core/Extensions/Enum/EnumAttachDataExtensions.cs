using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace CoolCode {
	[AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
	public class AttachDataAttribute : Attribute {
		public AttachDataAttribute(object key, object value) {
			this.Key = key;
			this.Value = value;
		}

		public object Key { get; private set; }

		public object Value { get; private set; }
	}

	public static class EnumAttachDataExtensions {
		private readonly static Dictionary<string, object> _cacheAttachedData = new Dictionary<string, object>();
		private readonly static object lockObject = new object();

		public static object GetAttachedData(this ICustomAttributeProvider provider, object key) {
			var attributes = (AttachDataAttribute[])provider.GetCustomAttributes(typeof(AttachDataAttribute), false);

			return attributes.First(a => a.Key.Equals(key)).Value;
		}

		public static T GetAttachedData<T>(this ICustomAttributeProvider provider, object key) {
			return (T)provider.GetAttachedData(key);
		}

		public static object GetAttachedData(this Enum en, object key) {
			object value;
			string enumFullName = GetEnumFullName(en, key);
			if (!_cacheAttachedData.TryGetValue(enumFullName, out value)) {
				lock (lockObject) {
					if (!_cacheAttachedData.TryGetValue(enumFullName, out value)) {
						value = en.GetType().GetField(en.ToString()).GetAttachedData(key);
						_cacheAttachedData.Add(enumFullName, value);
					}
				}
			}

			return value;
		}

		public static T GetAttachedData<T>(this Enum en, object key) {
			return (T)en.GetAttachedData(key);
		}

		private static string GetEnumFullName(Enum en, object key) {
			return en.GetType().FullName + "." + en + "." + key;
		}
	}

	#region Sample

	/* 
public enum AgeRange
{ 
	[AttachData(AgeRangeAttachData.Text, "18岁及以下")]
	LessThan18,

	[AttachData(AgeRangeAttachData.Text, "19至29岁")]
	From19To29,

	[AttachData(AgeRangeAttachData.Text, "30岁及以上")]
	Above29
}

public enum AgeRangeAttachData
{ 
	Text
}

public static class AgeRangeExtensions
{
	public static string GetText(this AgeRange range)
	{
		return range.GetAttachedData<string>(AgeRangeAttachData.Text);
	}
}

	 */
	#endregion
}
