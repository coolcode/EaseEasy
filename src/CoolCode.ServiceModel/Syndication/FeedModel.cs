using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Syndication;

namespace CoolCode.Web.Mvc.Syndication {
	public class FeedModel : FeedBase {
		public List<FeedCategory> Categories { get; set; }
		public List<FeedItem> Items { get; set; }

		public FeedModel() {
			Categories = new List<FeedCategory>();
			Items = new List<FeedItem>();
		}

	}

}
