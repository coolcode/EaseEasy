using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CoolCode;

namespace CoolCodeTest {
	[TestClass]
	public class EnumTest {
		[TestMethod]
		public void AttachStringTest() {
			var act = Chinglish.Niubility.GetAttachedString();
			var exp = "牛逼";
			Assert.AreEqual(exp, act);
		}

		[TestMethod]
		public void EnumQuery_AsEnumerable_Test() {
			var enumItems = Enum<Chinglish>.AsEnumerable();
			var exp = new Chinglish[] { Chinglish.Niubility, Chinglish.Zhuangbility, Chinglish.Shability, Chinglish.Erbility };
			foreach (var item in enumItems) {
				Assert.IsTrue(exp.Contains(item));
			}
		}

		[TestMethod]
		public void EnumQuery_Count_Test() {
			var act = Enum<Chinglish>.AsEnumerable().Count();
			var exp = 4;
			Assert.AreEqual(exp, act);
		}
	}

	/// <summary>
	/// 中国式英语
	/// </summary>
	enum Chinglish {
		[AttachString("牛逼")]
		Niubility,
		[AttachString("装逼")]
		Zhuangbility,
		[AttachString("傻逼")]
		Shability,
		[AttachString("二逼")]
		Erbility,
	}
}
