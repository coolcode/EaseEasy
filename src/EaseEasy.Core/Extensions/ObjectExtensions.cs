using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.ComponentModel;
using Newtonsoft.Json;

namespace EaseEasy {
	public static class ObjectExtensions {
		//ScottGu In扩展 改进
		public static bool In<T>(this T t, params T[] c) {
			return c.Contains(t);
			//return c.Any(i => i.Equals(t));
		}

		public static T Clone<T>(this T t) {
			return (T)CloneObject(t);
		}

		private static object CloneObject(object obj) {
			using (MemoryStream memStream = new MemoryStream()) {
				BinaryFormatter binaryFormatter = new BinaryFormatter(null, new StreamingContext(StreamingContextStates.Clone));
				binaryFormatter.Serialize(memStream, obj);
				memStream.Seek(0, SeekOrigin.Begin);
				return binaryFormatter.Deserialize(memStream);
			}
		}

		/// <summary>
		/// Throws an ArgumentNullException if the given data item is null.
		/// </summary>
		/// <param name="data">The item to check for nullity.</param>
		/// <param name="name">The name to use when throwing an exception, if necessary</param>
		public static void ThrowIfNull<T>(this T data, string name) where T : class {
			if (data == null) {
				throw new ArgumentNullException(name);
			}
		}

		/// <summary>
		/// Throws an ArgumentNullException if the given data item is null.
		/// No parameter name is specified.
		/// </summary>
		/// <param name="data">The item to check for nullity.</param>
		public static void ThrowIfNull<T>(this T data) where T : class {
			if (data == null) {
				throw new ArgumentNullException();
			}
		}


		public static void ThrowIfNullOrEmpty(this string data, string name) {
			if (string.IsNullOrEmpty(data)) {
				throw new ArgumentException(name);
			}
		}

		public static void ThrowIfNullOrEmpty(this IEnumerable data, string name) {
			if (data == null || !data.GetEnumerator().MoveNext()) {
				throw new ArgumentException(name);
			}
		}

		public static T ConvertTo<T>(this object value) {
			if (value == null) {
				return default(T);
			}

			if (typeof(T) == typeof(IDictionary<string, object>) || typeof(T).GetInterface("IDictionary`2") != null) {
				return (T)ConvertToDictionary(value);
			}

			TypeConverter tc = TypeDescriptor.GetConverter(typeof(T));

			if (tc.CanConvertFrom(value.GetType())) {
				return (T)tc.ConvertFrom(value);
			}
			else {
				return (T)Convert.ChangeType(value, typeof(T));
			}
		}


		/// <summary>
		/// Convert to unknow type object's property names and values into a Dictionary object.
		/// </summary>
		/// <param name="data">The object to be converted.</param>
		/// <returns>A Dictionary object contains the converted object's property names and values.</returns>
		public static IDictionary<string, object> ConvertToDictionary(this object data) {
			if (data is IDictionary<string, object>) {
				return data as IDictionary<string, object>;
			}

			var dict = new Dictionary<string, object>();
			foreach (var property in data.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)) {
				if (property.CanRead) {
					dict.Add(property.Name, property.GetValue(data, null));
				}
			}

			return dict;
		}
        
        public static string ToJson(this object data) {
            return JsonConvert.SerializeObject(data);
        }

        public static T ToObject<T>(this string text) {
            return JsonConvert.DeserializeObject<T>(text);
        }

        public static Stream ToStream(this string text) {
            byte[] array = Encoding.UTF8.GetBytes(text);

            return new MemoryStream(array);
        }

        public static string ToText(this Stream stream) {
            using (var reader = new StreamReader(stream)) {
                return reader.ReadToEnd();
            }
        }

        public static Stream ToStream(this object data) {
            return data.ToJson().ToStream();
        }

        public static T ToObject<T>(this Stream stream) {
            return stream.ToText().ToObject<T>();
        }   
	}
}
