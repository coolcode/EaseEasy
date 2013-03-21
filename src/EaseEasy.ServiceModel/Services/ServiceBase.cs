using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using EaseEasy.ServiceModel.Logging;

namespace EaseEasy.ServiceModel.Services {
    public abstract class ServiceBase {
        protected readonly DbContext db = DependencyResolver.Current.GetService<DbContext>();
        protected static readonly ILogger Logger = LogManager.GetLogger(typeof(ServiceBase));
    }

    public abstract class ServiceBase<T> : ServiceBase where T : DbContext, new() {
        private T _db = null;
        protected new T db {
            get {
                return _db ?? (_db = base.db as T ?? new T());
            }
        }
    }
}
