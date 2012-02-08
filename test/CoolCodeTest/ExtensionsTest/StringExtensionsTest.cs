using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CoolCode;
using CoolCode.Reflection;

namespace CoolCodeTest {
	[TestClass]
	public class StringExtensionsTest {
		[TestMethod]
		public void To_Int_Test() {
			var exp = 1024;
			var act = "1024".To<int>();
			Assert.AreEqual(exp, act);
		}

		[TestMethod]
		public void To_Int_Nullable_Test() {
			int? exp = 1024;
			var act = "1024".To<int?>();
			Assert.AreEqual(exp, act);
		}

		[TestMethod]
		public void To_Guid_Test() {
			var exp = Guid.NewGuid();
			var guid = exp.ToString();
			var act = guid.To<Guid>();
			Assert.AreEqual(exp, act);
		}

		[TestMethod]
		public void To_Object_Test() {
			Foo exp = new Foo() { Value = "Hello" };
			var act = "Hello".To<Foo>();
			Assert.AreEqual(exp, act);
		}

		public class Foo {
			public string Value { get; set; }
			public static Foo Parse(string value) {
				return new Foo() { Value = value };
			}

			public override bool Equals(object obj) {
				return ((Foo)obj).Value == Value;
			}
		}
	}

}
