using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Syndication;
using System.Xml;

namespace EaseEasy.Web.Mvc.Syndication {
	public class FeedReader {
		public FeedModel Read(string url) {
			var feed = new Rss20FeedFormatter();
			using (var xreader = XmlReader.Create(url)) {
				feed.ReadFrom(xreader);
			}

			return feed.Feed.ToFeedModel();
		}
	}


}
