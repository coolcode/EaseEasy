using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace EaseEasy.Reflection {
	/// <summary>
	/// 反射扩展
	/// </summary>
	public static class ReflectionExtensions {
		/// <summary>
		/// 是否只读
		/// </summary>
		/// <param name="member"></param>
		/// <returns></returns>
		public static bool IsReadOnly(this MemberInfo member) {
			switch (member.MemberType) {
				case MemberTypes.Field:
					return (((FieldInfo)member).Attributes & FieldAttributes.InitOnly) != 0;
				case MemberTypes.Property:
					PropertyInfo pi = (PropertyInfo)member;
					return !pi.CanWrite || pi.GetSetMethod() == null;
				default:
					return true;
			}
		}
	}
}
