using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using EaseEasy.ServiceModel.Security;
using EaseEasy.ServiceModel.Services;
using EaseEasy.ServiceModel.Services.Implement;
using Ninject.Modules;

namespace EaseEasy.ServiceModel.Mvc {
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
