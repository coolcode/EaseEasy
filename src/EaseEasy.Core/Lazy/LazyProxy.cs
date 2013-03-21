using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace EaseEasy {
	internal class LazyProxy<T> {
		public LazyProxy() {
			var asmName = new AssemblyName("Lazy-Assembly-" + Guid.NewGuid().ToString());
			var asmBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(asmName, AssemblyBuilderAccess.Run);
			var moduleBuilder = asmBuilder.DefineDynamicModule("Lazy-Module-" + Guid.NewGuid().ToString());
			var typeBuilder = moduleBuilder.DefineType(typeof(T).FullName + "$LazyProxy", TypeAttributes.Public, typeof(T));

			this.CreateConstructors(typeBuilder);
			var propertyMapping = this.CreateProperties(typeBuilder);
			this.PropertyCount = propertyMapping.Count;

			this.ProxyType = typeBuilder.CreateType();

			this.InitializeConstructorMapping();
			this.InitializeLazyLoaderSetterMapping(propertyMapping);
		}

		private Dictionary<PropertyInfo, object> _lazyLoaderSetterMapping;

		private void InitializeLazyLoaderSetterMapping(Dictionary<PropertyInfo, string> propertyMapping) {
			this._lazyLoaderSetterMapping = propertyMapping.ToDictionary(
				p => p.Key,
				p => this.CreateLazyLoaderSetter(this.ProxyType.GetField(p.Value)));
		}

		private object CreateLazyLoaderSetter(FieldInfo lazyLoader) {
			var setterType = typeof(Action<,>).MakeGenericType(typeof(T), typeof(object));

			var setter = new DynamicMethod("SetLazyLoader", typeof(void), new Type[] { typeof(T), typeof(object) }, this.ProxyType);
			var ilGenerator = setter.GetILGenerator();
			ilGenerator.Emit(OpCodes.Ldarg_0);
			ilGenerator.Emit(OpCodes.Castclass, this.ProxyType);
			ilGenerator.Emit(OpCodes.Ldarg_1);
			ilGenerator.Emit(OpCodes.Castclass, lazyLoader.FieldType);
			ilGenerator.Emit(OpCodes.Stfld, lazyLoader);
			ilGenerator.Emit(OpCodes.Ret);

			return setter.CreateDelegate(setterType);
		}

		private Dictionary<PropertyInfo, string> CreateProperties(TypeBuilder typeBuilder) {
			var result = new Dictionary<PropertyInfo, string>();

			var allProperties = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			foreach (var property in allProperties) {
				var lazyLoaderName = typeBuilder.CreatePropertyWithLazyLoader(property);
				if (String.IsNullOrEmpty(lazyLoaderName))
					continue;
				result.Add(property, lazyLoaderName);
			}

			return result;
		}

		public int PropertyCount { get; private set; }

		public bool HasDefaultConstructor { get; private set; }

		private void CreateConstructors(TypeBuilder typeBuilder) {
			var constructors = typeof(T).GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			this.HasDefaultConstructor = false;

			foreach (var ctor in constructors) {
				if (ctor.GetParameters().Length == 0) {
					this.HasDefaultConstructor = true;
				}

				typeBuilder.CreateConstructor(ctor);
			}
		}

		public Type ProxyType { get; private set; }

		public int ConstructorCount { get; private set; }

		private Dictionary<ConstructorInfo, ConstructorInfo> _baseConstructors;

		private static bool MatchConstructor(ConstructorInfo child, ConstructorInfo parent) {
			var parentParameters = parent.GetParameters();
			var childParameters = child.GetParameters();

			if (parentParameters.Length != childParameters.Length)
				return false;

			for (int i = 0; i < childParameters.Length; i++) {
				if (parentParameters[i].ParameterType != childParameters[i].ParameterType) {
					return false;
				}
			}

			return true;
		}

		private void InitializeConstructorMapping() {
			this._baseConstructors = new Dictionary<ConstructorInfo, ConstructorInfo>();
			//var childConstructors = this.ProxyType.GetConstructors();

			foreach (var parent in this.ProxyType.GetConstructors()) {
				foreach (var child in typeof(T).GetConstructors()) {
					if (MatchConstructor(child, parent)) {
						this._baseConstructors.Add(child, parent);
						break;
					}
				}
			}

			this.ConstructorCount = this._baseConstructors.Count;
		}

		public void SetLazyLoader(PropertyInfo property, T instance, object loader) {
			var setter = (Action<T, object>)this._lazyLoaderSetterMapping[property];
			setter(instance, loader);
		}
	}

	/* Sample:      
     
	public class LazyTypeTestClass
	{
		public LazyTypeTestClass()
		{
			this.ArgCountOfUsedConstructor = 0;
		}

		public LazyTypeTestClass(int i)
		{
			this.ArgCountOfUsedConstructor = 1;
		}

		public LazyTypeTestClass(int i, DateTime dt)
		{
			this.ArgCountOfUsedConstructor = 2;
		}

		public LazyTypeTestClass(int i0, int i1, int i2, int i3, int i4, DateTime dt)
		{
			this.ArgCountOfUsedConstructor = 6;
			this.DateTimeSetByConstructor = dt;
		}

		public DateTime DateTimeSetByConstructor;

		public int ArgCountOfUsedConstructor = -1;

		public int NonVirtualProperty { get; set; }

		public virtual int VirtualProperty { get; set; }

		public virtual int ReadOnlyProperty { get; private set; }
	}

	public class LazyProxyTest
	{
		[Fact]
		public void InitializeByProperConstructors()
		{
			var lazyProxy = new LazyProxy<LazyTypeTestClass>();
			Assert.NotNull(lazyProxy.ProxyType);
			Assert.Equal(4, lazyProxy.ConstructorCount);

			var byZero = (LazyTypeTestClass)Activator.CreateInstance(lazyProxy.ProxyType);
			Assert.Equal(0, byZero.ArgCountOfUsedConstructor);

			var byOne = (LazyTypeTestClass)Activator.CreateInstance(lazyProxy.ProxyType, 1);
			Assert.Equal(1, byOne.ArgCountOfUsedConstructor);

			var byTwo = (LazyTypeTestClass)Activator.CreateInstance(lazyProxy.ProxyType, 1, DateTime.Now);
			Assert.Equal(2, byTwo.ArgCountOfUsedConstructor);

			var dt = DateTime.Now;
			var bySix = (LazyTypeTestClass)Activator.CreateInstance(lazyProxy.ProxyType, 1, 2, 3, 4, 5, DateTime.Now);
			Assert.Equal(6, bySix.ArgCountOfUsedConstructor);
			Assert.Equal(dt, bySix.DateTimeSetByConstructor);
		}

		[Fact]
		public void OverrideProperProperties()
		{
			var lazyProxy = new LazyProxy<LazyTypeTestClass>();
			Assert.NotNull(lazyProxy.ProxyType);
			Assert.Equal(1, lazyProxy.PropertyCount);

			var instance = (LazyTypeTestClass)Activator.CreateInstance(lazyProxy.ProxyType);
			instance.VirtualProperty = 1;
			Assert.Equal(1, instance.VirtualProperty);
		}

		[Fact]
		public void LazyLoading()
		{
			var lazyProxy = new LazyProxy<LazyTypeTestClass>();
			var instance = (LazyTypeTestClass)Activator.CreateInstance(lazyProxy.ProxyType);

			Expression<Func<LazyTypeTestClass, int>> expr = c => c.VirtualProperty;
			var property = (expr.Body as MemberExpression).Member as PropertyInfo;

			int count = 0;
			instance.NonVirtualProperty = count;
			lazyProxy.SetLazyLoader(property, instance, (Func<int>)(() => count));
			count = 10;

			Assert.Equal(0, instance.NonVirtualProperty);
			Assert.Equal(10, instance.VirtualProperty);

			count = 100;
			Assert.Equal(10, instance.VirtualProperty);
		}
	}
	 */
}