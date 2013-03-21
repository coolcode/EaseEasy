using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Ninject;

namespace EaseEasy.ServiceModel.Mvc {
	public class NInjectDependencyResolver : IDependencyResolver {
		private IKernel _kernel;

		public NInjectDependencyResolver(IKernel kernel) {
			_kernel = kernel;
		}

		#region IDependencyResolver Members

		public object GetService(Type serviceType) {
			return _kernel.TryGet(serviceType);
		}

		public IEnumerable<object> GetServices(Type serviceType) {
			return _kernel.GetAll(serviceType);
		}

		#endregion
	}
}
