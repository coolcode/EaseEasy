using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Web;
using System.Web.Caching;
using System.Web.Hosting;
using System.Web.Mvc;
using CoolCode.ServiceModel.Services;

namespace CoolCode.ServiceModel.Mvc.Hosting {
	[AspNetHostingPermission(SecurityAction.Demand, Level = AspNetHostingPermissionLevel.Medium)]
	[AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.High)]
	public class DbVirtualPathProvider : VirtualPathProvider {

		private static readonly IVirtualViewService _virtualViewService = DependencyResolver.Current.GetService<IVirtualViewService>();

		public DbVirtualPathProvider(string database = null) {
			if (!string.IsNullOrEmpty(database) && _virtualViewService.CacheDependency == null) {
				_virtualViewService.CacheDependency = new SqlCacheDependency(database, "VirtualViews");
			}
		}

		public override bool DirectoryExists(string virtualDir) {
			if (_virtualViewService.PathExists(virtualDir)) {
				return true;
			}

			return base.DirectoryExists(virtualDir);
		}

		public override bool FileExists(string virtualPath) {
			if (_virtualViewService.PathExists(virtualPath)) {
				return true;
			}

			return base.FileExists(virtualPath);
		}

		public override VirtualFile GetFile(string virtualPath) {
			if (_virtualViewService.PathExists(virtualPath)) {
				VirtualFile virtualFile = new DbVirtualFile(virtualPath, _virtualViewService);
				return virtualFile;
			}

			var file = base.GetFile(virtualPath);
			return file;
		}

		public override CacheDependency GetCacheDependency(string virtualPath, IEnumerable virtualPathDependencies, DateTime utcStart) {
			if (_virtualViewService.PathExists(virtualPath)) {
				return _virtualViewService.CacheDependency;
			}

			return base.GetCacheDependency(virtualPath, virtualPathDependencies, utcStart);
		}
	}
}
