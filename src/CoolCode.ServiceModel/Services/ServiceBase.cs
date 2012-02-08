using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace CoolCode.ServiceModel.Services {
	public abstract class ServiceBase {
		protected DbContext db = DependencyResolver.Current.GetService<DbContext>();
	}
}
