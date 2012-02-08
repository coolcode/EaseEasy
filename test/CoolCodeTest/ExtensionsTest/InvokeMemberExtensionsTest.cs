using System;
using System.Linq.Expressions;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CoolCode;

namespace CoolCodeTest {
	[TestClass]
	public class InvokeMemberExtensionsTest {
		[TestMethod]
		public void SetValueTest() {
			Foo foo = new Foo();
			foo.SetValue("Property", "Hello!");
			Assert.AreEqual("Hello!", foo.Property);

			dynamic dy = new Foo();
			InvokeMemberExtensions.SetValue(dy, "Property", "Hello!");
			Assert.AreEqual("Hello!", dy.Property);
		}

		[ExpectedException(typeof(MethodAccessException))]
		[TestMethod]
		public void SetValue_Exception_Test() {
			Foo foo = new Foo();
			foo.SetValue("abc", "Hello!");
		}

		[TestMethod]
		public void GetValueTest() {
			Foo foo = new Foo { Property = "Hello!" };
			var act = foo.GetValue("Property");
			Assert.AreEqual("Hello!", act);

			dynamic dy = new Foo { Property = "Hello!" };
			act = InvokeMemberExtensions.GetValue(dy, "Property");
			Assert.AreEqual("Hello!", act);
		}

		[TestMethod]
		public void AnonymousType_GetValueTest() {
			var non = new { Prop = "Hello!" };
			var act = non.GetValue("Prop");
			Assert.AreEqual("Hello!", act);
		}

		[TestMethod]
		public void AnonymousType_GetNullValueTest() {
			var non = new { Prop = (int?)null };
			var act = non.GetValue("Prop");
			Assert.AreEqual(null, act);
		}

		[ExpectedException(typeof(MethodAccessException))]
		[TestMethod]
		public void GetValue_Exception_Test() {
			Foo foo = new Foo { Property = "Hello!" };
			foo.GetValue("abc");
		}

		[TestMethod]
		public void DuplicateSetAndGetValueTest() {
			Foo foo = new Foo();
			foo.SetValue("Property", "Hello!");
			Assert.AreEqual("Hello!", foo.Property);

			var act = foo.GetValue("Property");
			Assert.AreEqual("Hello!", act);

			foo.SetValue("Property", "Hello2!");
			Assert.AreEqual("Hello2!", foo.Property);

			act = foo.GetValue("Property");
			Assert.AreEqual("Hello2!", act);
		}

		[TestMethod]
		public void ComplexType_SetValueTest() {
			Bar bar = new Bar();
			Foo foo = new Foo { Property = "Hello!" };
			bar.SetValue("Foo", foo);
			Assert.AreEqual(foo, bar.Foo);
		}

		[TestMethod]
		public void ComplexType_GetValueTest() {
			Bar bar = new Bar {
				Foo = new Foo { Property = "Hello!" }
			};

			var act = bar.GetValue("Foo");
			Assert.AreEqual(bar.Foo, act);
			Assert.AreEqual(bar.Foo.Property, ((Foo)act).Property);
		}

		class Foo {
			public string Property { get; set; }
		}


		class Bar {
			public Foo Foo { get; set; }
		}
	}
}
