using System.Collections.Generic;
using System.Web.Caching;

namespace CoolCode.ServiceModel.Services {
	public interface IVirtualViewService {
		CacheDependency CacheDependency { get; set; }
		bool PathExists(string virtualPath);
		IList<VirtualView> GetViews();
	}
}
