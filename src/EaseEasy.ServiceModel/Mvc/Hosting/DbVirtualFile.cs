using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Hosting;
using EaseEasy.ServiceModel.Services;

namespace EaseEasy.ServiceModel.Mvc.Hosting {
	public class DbVirtualFile : VirtualFile {

		public IVirtualViewService VirtualViewService { get; private set; }

		public DbVirtualFile(string virtualPath, IVirtualViewService virtualViewService)
			: base(virtualPath) {
			this.VirtualViewService = virtualViewService;
		}

		public override Stream Open() {
			var views = VirtualViewService.GetViews();
			var content = views.Where(c => c.Path.Equals(this.VirtualPath, StringComparison.InvariantCultureIgnoreCase)).Select(c => c.Html).FirstOrDefault();

			var bytes = Encoding.UTF8.GetBytes(content);
			var memoryStream = new MemoryStream(bytes);
			memoryStream.Seek(0, SeekOrigin.Begin);

			return memoryStream;
		}
	}
}
