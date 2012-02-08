using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using CoolCode.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CoolCodeTest {
	/// <summary>
	/// Summary description for CollectionTest
	/// </summary>
	[TestClass]
	public class CollectionTest { 
		[TestMethod]
		public void TestMethod1() {
			var items = new List<Foo> {
				new Foo(DateTime.Parse("2010-1-2")),
				new Foo(),
				new Foo(DateTime.Parse("2010-1-4")),
				new Foo(DateTime.Parse("2012-2-2")),
				new Foo(DateTime.Parse("2011-4-2")),
				new Foo()
			};

			var relatedItems = items.AsRelationCollection();

			foreach (var item in relatedItems) {
				if(item.IsFirst) {
					//Do sth...
				}
				if (item.IsLast) {
					//Do sth...
				} 
			}
		}

		class Foo {
			private static int id = 0;
			public Foo(DateTime? date = null) {
				Id = id++;
				Date = date;
			}
			public int Id { get; set; }
			public DateTime? Date { get; set; }
		}
	}
}
