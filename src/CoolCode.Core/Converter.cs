using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace CoolCode {
	public interface IConverter<T> {
		T Serialize(object o);
		object Deserialize(T value, Type objectType);
	}

	public class StringConverter : IConverter<string> {
		private readonly static Dictionary<Type, TypeConverter> converters = new Dictionary<Type, TypeConverter>();

		#region IConverter<string> Members

		public virtual string Serialize(object o) {
			if (o == null)
				return string.Empty;
			Type objectType = o.GetType();
			if (objectType == typeof(string))
				return (string)o;
			if (objectType == typeof(DateTime))
				return ((DateTime)o).ToString("yyyy-MM-dd hh:mm:ss.ffffff");

			converters.AddIfNotExists(objectType, TypeDescriptor.GetConverter(objectType));

			string value = converters[objectType].ConvertToString(o);
			return value;

		}

		public virtual object Deserialize(string value, Type objectType) {
			converters.AddIfNotExists(objectType, TypeDescriptor.GetConverter(objectType));

			object o = converters[objectType].ConvertFromString(value);
			return o;
		}

		#endregion
	}

	public class Converter<T> : IConverter<T> {
		private IConverter<T> converter;
		public readonly static IConverter<T> Default = new Converter<T>();

		public Converter() {
			converter = ConverterFactory.Provide<T>();
		}

		public T Serialize(object o) {
			return converter.Serialize(o);
		}

		public object Deserialize(T value, Type objectType) {
			return converter.Deserialize(value, objectType);
		}
	}

	public static class ConverterFactory {
		private readonly static Dictionary<Type, object> converters = new Dictionary<Type, object>();

		static ConverterFactory() {
			converters.Add(typeof(string), new StringConverter());
		}

		public static IConverter<T> Provide<T>() {
			IConverter<T> converter = null;
			Type t = typeof(T);

			if (converters.ContainsKey(t)) {
				converter = converters[t] as IConverter<T>;
			}
			if (converter == null) {
				throw new NotSupportedException("Cannot found the converter of " + t.FullName);
			}

			return converter;
		}
	}
}
