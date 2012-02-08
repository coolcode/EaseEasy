using System.ServiceModel.Syndication;
using System.Web.Mvc;
using System.Xml;

namespace CoolCode.Web.Mvc
{
	public enum FeedType
	{
		Rss,
		Atom
	}

	public sealed class FeedResult : ActionResult
	{
		public SyndicationFeed Feed { get; set; }

		public FeedType Type { get; set; }

		public FeedResult(SyndicationFeed feed, FeedType type)
		{
			this.Feed = feed;
			this.Type = type;
		}

		public override void ExecuteResult(ControllerContext context)
		{ 
			context.HttpContext.Response.ContentType = "text/xml;charset=utf-8";
			if (Type == FeedType.Rss)
			{
				Rss20FeedFormatter rssFormat = new Rss20FeedFormatter(Feed);
				XmlWriter rssWriter = new XmlTextWriter(context.HttpContext.Response.OutputStream, System.Text.Encoding.UTF8);
				rssFormat.WriteTo(rssWriter);
				rssWriter.Close();
			}
			else if (Type == FeedType.Atom)
			{
				Atom10FeedFormatter atomFormat = new Atom10FeedFormatter(Feed);
				XmlWriter atomWriter = new XmlTextWriter(context.HttpContext.Response.OutputStream, System.Text.Encoding.UTF8);
				atomFormat.WriteTo(atomWriter);
				atomWriter.Close();
			}
		}
	}
}
