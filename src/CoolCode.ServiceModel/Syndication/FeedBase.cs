using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoolCode.Web.Mvc.Syndication {
	public class FeedBase {
		public string ID { get; set; }
		public string Url { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }
		public string UserName { get; set; }
		public string Email { get; set; }
		public string Text { get; set; }
	}
}
