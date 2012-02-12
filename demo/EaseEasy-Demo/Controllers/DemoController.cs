using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EaseEasy_Demo.Models;

namespace EaseEasy_Demo.Controllers {
	public class DemoController : Controller {
		public ActionResult Index() {
			var db = new DemoContext();

			return View(db.Blogs);
		}

		public ActionResult AjaxGrid() {
			return View();
		}

		public ActionResult ItemList() {
			var db = new DemoContext();

			return View(db.Blogs);
		}

	}
}
