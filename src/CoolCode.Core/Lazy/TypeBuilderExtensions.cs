using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace CoolCode {
	internal static class TypeBuilderExtensions {
		private static readonly OpCode[] LoadArgsOpCodes = new OpCode[] { OpCodes.Ldarg_1, OpCodes.Ldarg_2, OpCodes.Ldarg_3 };

		public static void CreateConstructor(this TypeBuilder typeBuilder, ConstructorInfo baseConstructor) {
			if (baseConstructor == null)
				return;
			if (!baseConstructor.IsPublic && baseConstructor.IsFamily)
				return;

			var parameters = baseConstructor.GetParameters();
			if (parameters.Length == 0) {
				typeBuilder.DefineDefaultConstructor(MethodAttributes.Public);
				return;
			}

			var builder = typeBuilder.DefineConstructor(
				MethodAttributes.Public,
				CallingConventions.HasThis,
				parameters.Select(p => p.ParameterType).ToArray());

			var ilGenerator = builder.GetILGenerator();
			ilGenerator.Emit(OpCodes.Ldarg_0); // load "this"

			for (int i = 0; i < parameters.Length; i++) {
				if (i < LoadArgsOpCodes.Length) {
					ilGenerator.Emit(LoadArgsOpCodes[i]);
				}
				else {
					ilGenerator.Emit(OpCodes.Ldarg_S, i + 1);
				}
			}

			ilGenerator.Emit(OpCodes.Call, baseConstructor);
			ilGenerator.Emit(OpCodes.Ret);
		}

		public static string CreatePropertyWithLazyLoader(this TypeBuilder typeBuilder, PropertyInfo baseProperty) {
			if (baseProperty == null)
				return null;
			if (baseProperty.GetIndexParameters().Length > 0)
				return null;
			if (!baseProperty.CanWrite || !baseProperty.CanRead)
				return null;

			Func<MethodInfo, bool> isOverridable = m => {
				if (!m.IsVirtual)
					return false;
				return (m.IsPublic || m.IsFamily);
			};

			if (!isOverridable(baseProperty.GetGetMethod(true)) || !isOverridable(baseProperty.GetSetMethod(true)))
				return null;

			var lazyLoaderName = baseProperty.Name + "$LazyLoader";
			var lazyLoaderType = typeof(Func<>).MakeGenericType(baseProperty.PropertyType);

			var lazyLoader = typeBuilder.DefineField(lazyLoaderName, lazyLoaderType, FieldAttributes.Public);

			var ma = MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.Virtual;

			// define setter
			var baseSetter = baseProperty.GetSetMethod(true);
			var setterMa = ma | (baseSetter.IsPublic ? MethodAttributes.Public : MethodAttributes.Family);
			var setterBuilder = typeBuilder.DefineMethod("set_" + baseProperty.Name, setterMa, typeof(void), new Type[] { baseProperty.PropertyType });
			var setterIL = setterBuilder.GetILGenerator();
			// set lazy loader to null
			setterIL.Emit(OpCodes.Ldarg_0);
			setterIL.Emit(OpCodes.Ldnull);
			setterIL.Emit(OpCodes.Stfld, lazyLoader);
			// set base property
			setterIL.Emit(OpCodes.Ldarg_0);
			setterIL.Emit(OpCodes.Ldarg_1);
			setterIL.Emit(OpCodes.Call, baseSetter);
			// return
			setterIL.Emit(OpCodes.Ret);

			// define getter
			var baseGetter = baseProperty.GetGetMethod(true);
			var getterMa = ma | (baseGetter.IsPublic ? MethodAttributes.Public : MethodAttributes.Family);
			var getterBuilder = typeBuilder.DefineMethod("get_" + baseProperty.Name, getterMa, baseProperty.PropertyType, new Type[0]);
			var getterIL = getterBuilder.GetILGenerator();
			// label to return
			var returnLabel = getterIL.DefineLabel();
			// jump to return if lazy loader is null
			getterIL.Emit(OpCodes.Ldarg_0);
			getterIL.Emit(OpCodes.Ldfld, lazyLoader);
			getterIL.Emit(OpCodes.Brfalse_S, returnLabel);
			// invoke lazy loader and set to base
			getterIL.Emit(OpCodes.Ldarg_0);
			getterIL.Emit(OpCodes.Ldarg_0);
			getterIL.Emit(OpCodes.Ldfld, lazyLoader);
			getterIL.Emit(OpCodes.Callvirt, lazyLoaderType.GetMethod("Invoke"));
			getterIL.Emit(OpCodes.Call, baseSetter);
			// set lazy loader to null
			getterIL.Emit(OpCodes.Ldarg_0);
			getterIL.Emit(OpCodes.Ldnull);
			getterIL.Emit(OpCodes.Stfld, lazyLoader);
			// mark return label and return base value;
			getterIL.MarkLabel(returnLabel);
			getterIL.Emit(OpCodes.Ldarg_0);
			getterIL.Emit(OpCodes.Call, baseGetter);
			getterIL.Emit(OpCodes.Ret);

			return lazyLoaderName;
		}
	}
}
