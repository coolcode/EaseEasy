using System;
using System.Collections;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CoolCode;

namespace CoolCodeTest {
	/// <summary>
	/// Summary description for ObjectExtensionsTest
	/// </summary>
	[TestClass]
	public class ObjectExtensionsTest {
		[TestMethod]
		public void ConvertTo_Dictionary_Test() {
			object obj = new { Name = "Bruce", Age = 10 };
			var act = obj.ConvertTo<IDictionary<string, object>>().ToDictionary(c => c.Key, v => v.Value);
			var exp = new Dictionary<string, object> { { "Name", "Bruce" }, { "Age", 10 } };

			CollectionAssert.AreEqual(exp, act);
		}

	}
}
