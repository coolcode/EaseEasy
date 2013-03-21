using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

[assembly: InternalsVisibleTo("EaseEasyTest")]
namespace EaseEasy.Reflection {
	internal class TypeParser {
		private readonly static IDictionary<Type, FastInvokeHandler> methodCache = new Dictionary<Type, FastInvokeHandler>() {
		     {typeof(Guid), (obj, param) => new Guid((string) param[0]) }                                                              	
		};

		private readonly static object lockObject = new object();

		public static object Parse(Type type, string value) {
			type = type.GetNonNullableType();

			FastInvokeHandler parseHandler;
			if (!methodCache.TryGetValue(type, out parseHandler)) {
				lock (lockObject) { 
					if (!methodCache.TryGetValue(type, out parseHandler)) {
						var parseMethod = type.GetMethod("Parse", new Type[] { typeof(string) } );
						if (parseMethod == null) {
							throw new NotSupportedException("Parse(string) method cannot found in " + type.Name);
						}
						parseHandler = parseMethod.GetFastInvoker();
						methodCache.Add(type, parseHandler);
					}
				}
			}

			return parseHandler(null, value);
		}
	}
}
