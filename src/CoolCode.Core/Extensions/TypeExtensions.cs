using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace CoolCode 
{
    /// <summary>
    /// Type related extension methods
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        /// 基本数据类型
        /// </summary>
        private static readonly Type[] PrimitiveTypes = {
            typeof(byte),typeof(short), typeof(int), typeof(long),typeof(sbyte), typeof(ushort), typeof(uint),
            typeof(ulong), typeof(float), typeof(double), typeof(decimal),typeof(char), typeof(string),
            typeof(DateTime), typeof(Guid)
        };

        /// <summary>
        /// 查找IEnumerable&lt;T&gt;类型
        /// </summary>
        /// <param name="seqType"></param>
        /// <returns></returns>
        public static Type FindIEnumerable(this Type seqType)
        {
            if (seqType == null || seqType == typeof(string))
                return null;
            if (seqType.IsArray)
                return typeof(IEnumerable<>).MakeGenericType(seqType.GetElementType());
            if (seqType.IsGenericType)
            {
                foreach (Type arg in seqType.GetGenericArguments())
                {
                    Type ienum = typeof(IEnumerable<>).MakeGenericType(arg);
                    if (ienum.IsAssignableFrom(seqType))
                    {
                        return ienum;
                    }
                }
            }
            Type[] ifaces = seqType.GetInterfaces();
            if (ifaces != null && ifaces.Length > 0)
            {
                foreach (Type iface in ifaces)
                {
                    Type ienum = FindIEnumerable(iface);
                    if (ienum != null) return ienum;
                }
            }
            if (seqType.BaseType != null && seqType.BaseType != typeof(object))
            {
                return FindIEnumerable(seqType.BaseType);
            }
            return null;
        }

        /// <summary>
        /// 获取IEnumerable&lt;T&gt;类型
        /// </summary>
        /// <param name="elementType"></param>
        /// <returns></returns>
        public static Type GetSequenceType(this Type elementType)
        {
            return typeof(IEnumerable<>).MakeGenericType(elementType);
        }

        /// <summary>
        /// 如果是列表结构，则返回列表结构的元素类型，否则返回自身
        /// </summary>
        /// <param name="seqType"></param>
        /// <returns></returns>
        public static Type GetElementTypeOrSelf(this Type seqType)
        {
            //Type ienum = FindIEnumerable(seqType);
            //if (ienum == null) return seqType;
            //return ienum.GetGenericArguments()[0];

            /*Update:2009-12-13*/
            return seqType.GetElementType() ?? seqType;
        }

        /// <summary>
        /// 是否Nullable&lt;T&gt;类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsNullableType(this Type type)
        {
            return type != null && type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        /// <summary>
        /// 判断该类型是否能够赋予null
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsNullAssignable(this Type type)
        {
            return !type.IsValueType || IsNullableType(type);
        }

        /// <summary>
        /// 如果类型是Nullable&lt;T&gt;，则返回T，否则返回自身
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Type GetNonNullableType(this Type type)
        {
            if (IsNullableType(type))
            {
                return type.GetGenericArguments()[0];
            }
            return type;
        }

        /// <summary>
        /// 如果类型不是Nullable&lt;T&gt;，则返回Nullable&lt;T&gt;，否则返回自身
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Type GetNullAssignableType(this Type type)
        {
            if (!IsNullAssignable(type))
            {
                return typeof(Nullable<>).MakeGenericType(type);
            }
            return type;
        }       

        /// <summary>
        /// 获取MemberInfo的类型：字段、属性、事件、方法
        /// </summary>
        /// <param name="mi"></param>
        /// <returns></returns>
        public static Type GetMemberType(MemberInfo mi)
        {
            FieldInfo fi = mi as FieldInfo;
            if (fi != null) return fi.FieldType;
            PropertyInfo pi = mi as PropertyInfo;
            if (pi != null) return pi.PropertyType;
            EventInfo ei = mi as EventInfo;
            if (ei != null) return ei.EventHandlerType;
            MethodInfo meth = mi as MethodInfo;  // property getters really
            if (meth != null) return meth.ReturnType;
            return null;
        }

        /// <summary>
        /// 获取类型默认值
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object GetDefault(this Type type)
        {
            bool isNullable = !type.IsValueType || IsNullableType(type);
            if (!isNullable)
                return Activator.CreateInstance(type);
            return null;
        }

        /// <summary>
        /// 是否整数类型（包括byte，int，long）
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsInteger(this Type type)
        {
            Type nnType = GetNonNullableType(type);
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.SByte:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Byte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// 是否基本数据类型 (除了框架自带的以外，还包括 Decimal, String, DateTime, Guid)
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsPrimitive(this Type type)
        {
            return PrimitiveTypes.Contains(type);
        }

		/// <summary>
		/// 判断是否匿名类型
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static bool IsAnonymous(this Type type)
		{
			if (type == null)
				throw new ArgumentNullException("type");
 
			return Attribute.IsDefined(type, typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute), false)
					   && type.IsGenericType && type.Name.Contains("AnonymousType")
					   && (type.Name.StartsWith("<>") || type.Name.StartsWith("VB$"))
					   && (type.Attributes & TypeAttributes.NotPublic) == TypeAttributes.NotPublic;

		}
    }
}
