using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Web.Caching;
using System.Web.Mvc;
using EaseEasy.Caching;

namespace EaseEasy.ServiceModel.Services.Implement {
	public class DbVirtualViewService : IVirtualViewService {
		private const string VirtualViewsCacheKey = "VirtualViewService-Cache";
		private static readonly ICache viewCache = new WebCacheStrategy();

		public CacheDependency CacheDependency { get; set; }
		public string[] FileExtensions { get; private set; }

		public DbVirtualViewService() {
			FileExtensions = (from v in ViewEngines.Engines
							  let vppv = v as VirtualPathProviderViewEngine
							  where vppv != null
							  select vppv)
							  .SelectMany(c => c.FileExtensions)
							  .ToArray();
		}

		public bool PathExists(string virtualPath) {
			//仅判断Views路径下的视图文件
			if (!virtualPath.StartsWith("/Views") ||
				!(FileExtensions != null && FileExtensions.Any(ext => virtualPath.EndsWith("." + ext)))) {
				return false;
			}
			var paths = GetViews();

			return paths.Any(c => c.Path == virtualPath);
		}

		public IList<VirtualView> GetViews() {
			return viewCache.Get(VirtualViewsCacheKey, new SqlCacheDependency("xToDoList", "VirtualViews"),
								 () => {
									 var db = DependencyResolver.Current.GetService<DbContext>();
									 return db.Set<VirtualView>().ToList();
								 });
		}
	}
}
