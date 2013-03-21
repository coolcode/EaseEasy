using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EaseEasy.Web.Mvc.Syndication {
	public class FeedItem : FeedBase {
		public DateTime PublishDate { get; set; }
		public DateTime LastUpdatedTime { get; set; }
		public FeedCategory Category { get; set; }
	}
}
