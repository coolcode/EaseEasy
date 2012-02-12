using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace EaseEasy_Demo.Models {
	public class DemoContext : DbContext {
		public DbSet<Blog> Blogs { get; set; }
		public static void SetInitializer() {
			Database.SetInitializer(new DemoContextInitializer());
		}

		class DemoContextInitializer : DropCreateDatabaseIfModelChanges<DemoContext> {
			protected override void Seed(DemoContext context) {
				for (int i = 0; i < 102; i++) {
					context.Blogs.Add(new Blog {
						 Title = "MVC 框架"+i,
						 Author = "Bruce Lee"
					});
				}
			}
		}
	}
}