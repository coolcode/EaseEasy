using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using CoolCode.ServiceModel.Security;
using CoolCode.ServiceModel.Services;
using CoolCode.ServiceModel.Services.Implement;
using Ninject.Modules;

namespace CoolCode.ServiceModel.Mvc {
	public class DefaultModule : NinjectModule {
		public override void Load() {
			Bind<IDictionaryService>().To<DictionaryService>();
			Bind<IMembership>().To<DefaultMembership>();
			Bind<IFormsAuthentication>().To<DefaultFormsAuthentication>();
			Bind<DbContext>().To<DefaultDbContext>();
			Bind<IEntityWatcher>().To<DefaultEntityWatcher>();
			Bind<IVirtualViewService>().To<DbVirtualViewService>();
		}
	}
}
