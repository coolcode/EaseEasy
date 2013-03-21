using System;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;

namespace EaseEasy.Web.Mvc.Html
{
	public static class CacheExtensions
	{
		public static IHtmlString Cache(
			this HtmlHelper htmlHelper,
			string cacheKey,
			DateTime absoluteExpiration,
			Func<dynamic, object> func)
		{
			return htmlHelper.Cache(
				cacheKey,
				null,
				absoluteExpiration,
				System.Web.Caching.Cache.NoSlidingExpiration,
				func);
		}

		public static IHtmlString Cache(
			this HtmlHelper htmlHelper,
			string cacheKey,
			CacheDependency cacheDependencies,
			DateTime absoluteExpiration,
			TimeSpan slidingExpiration,
			Func<dynamic, object> func)
		{
			cacheKey.ThrowIfNullOrEmpty("cacheKey");
			func.ThrowIfNull("func");

			var cache = htmlHelper.ViewContext.HttpContext.Cache;
			var content = cache.Get(cacheKey) as string;

			if (content == null)
			{
				var result = func(htmlHelper.ViewContext);
				if (result != null)
				{
					content = result.ToString();
					cache.Insert(cacheKey, content, cacheDependencies, absoluteExpiration, slidingExpiration);
				}
			}

			return MvcHtmlString.Create(content);
		}
	}
}
