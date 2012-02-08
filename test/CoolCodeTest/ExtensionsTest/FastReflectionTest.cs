using System;
using System.Reflection;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CoolCode.Reflection;

namespace CoolCodeTest {
	[TestClass]
	public class FastReflectionTest {
		[TestMethod]
		public void FastInvokeMethodTest() {
			MethodInfo methodInfo = typeof(Person).GetMethod("Say");
			var fastInvoker = methodInfo.GetFastInvoker();
			fastInvoker(new Person(), new object[] { "hello" });
			var act = methodInfo.Invoke(new Person(), new object[] { "hello" });
			var exp = "Person Say:hello";
			Assert.AreEqual(exp,act);
		}
	}

	class Person {
		public string Say(string msg) {
			return "Person Say:" + msg;
		}
	}
}
