using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CoolCode.Security;

namespace CoolCodeTest {
	[TestClass]
	public class SecurityTest {
		[TestMethod]
		public void EncryptTest() {
			var exp = "Hello world!中文测试！";
			var act = exp.Encrypt();
			Assert.AreNotEqual(exp, act);

			act = act.Decrypt();
			Assert.AreEqual(exp, act);
		}
	}
}
