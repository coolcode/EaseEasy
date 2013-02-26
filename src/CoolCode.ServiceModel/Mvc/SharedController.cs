using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Metadata.Edm;
using System.Data.SqlClient;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using System.Xml.Linq;
using CoolCode.Linq;
using CoolCode.Linq.Dynamic;
using CoolCode.Web.Mvc;
using CoolCode.ServiceModel.Services;

namespace CoolCode.ServiceModel.Mvc {
    public class SharedController<T> : SharedController where T : DbContext {
        protected new readonly T db = (T)CurrentResolver.GetService<DbContext>();
    }

    public class SharedController : ControllerBase {
        #region Services

        protected readonly DbContext db = CurrentResolver.GetService<DbContext>();

        protected readonly IEntityWatcher EntityWatcher = CurrentResolver.GetService<IEntityWatcher>();

        protected override void Dispose(bool disposing) {
            db.Dispose();
            base.Dispose(disposing);
        }

        #endregion

        #region 通用CRUD

        public virtual ActionResult ItemList(string entity) {
            Type entityType = GetEntityType(entity);

            IQueryable model = QueryItems(db.Set(entityType));

            return View(model);
        }

        protected IQueryable<T> QueryItems<T>(IQueryBuilder<T> queryBuilder) where T : class {
            IQueryable<T> model = db.Set<T>();

            if (queryBuilder != null) {
                model = model.Where(queryBuilder.Expression);
            }

            model = QueryItems(model);

            return model;
        }

        protected IQueryable<T> QueryItems<T>(IQueryable<T> model) {
            return (IQueryable<T>)QueryItems((IQueryable)model);
        }

        protected IQueryable QueryItems(IQueryable model) {
            foreach (var key in this.ControllerContext.HttpContext.Request.Form.Keys.Cast<string>()) {
                if (key.Contains("@0")) {
                    var value = this.ControllerContext.HttpContext.Request.Form[key];
                    if (!string.IsNullOrEmpty(value)) {
                        var predicate = string.Format("{0}", key);
                        model = model.Where(predicate, value);
                    }
                }
            }
            /*
            if (model.ElementType.IsSubclassOf(typeof(TrackEntityBase))) {
                model = model.Where("it.CreateUserId = @0", this.UserID)
                    .OrderBy("it.CreateTime desc");
            }*/

            return model;
        }

        public virtual ActionResult New(string entity) {
            Type entityType = GetEntityType(entity);

            object model = GetInstanceByType(entityType);

            SetValue(model, "CreateUserId", this.UserID);
            SetValue(model, "CreateTime", DateTime.Now);

            ViewBag.FormMode = FormMode.New;
            return View("Edit", model);
        }

        [HttpPost]
        public virtual ActionResult New(string entity, FormCollection form) {
            Type entityType = GetEntityType(entity);

            AddEntity(entityType);

            ViewBag.FormMode = FormMode.New;
            return this.Success();
        }

        public virtual ActionResult Edit(string entity, string id) {
            Type entityType = GetEntityType(entity);

            object model = FindEntity(entityType,id);

            ViewBag.FormMode = FormMode.Edit;
            return View(model);
        }


        [HttpPost]
        public virtual ActionResult Edit(string entity, string id, FormCollection form) {
            Type entityType = GetEntityType(entity);

            UpdateEntity(entityType);

            ViewBag.FormMode = FormMode.Edit;
            return this.Success();
        }

        private bool TryUpdateModel(Type entityType, object model) {
            var method = typeof(Controller).GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
                            .First(c => c.Name == "TryUpdateModel" && c.GetParameters().Length == 1);

            method = method.MakeGenericMethod(entityType);

            object r = method.Invoke(this, new object[] { model });

            return (bool)r;
        }

        public virtual ActionResult Delete(string entity, string id) {
            Type entityType = GetEntityType(entity);
            DeleteEntity(entityType);

            return this.Success();
        }

        public virtual ActionResult BatchDelete(string entity, string id) {
            Type entityType = GetEntityType(entity);

            var keyProperty = GetKeyProperties(entityType).FirstOrDefault();

            //用参数传值防止SQL注入 
            var parameters = id.Split(',')
                .Select((c, i) => new SqlParameter("@p" + i, Convert.ChangeType(c, keyProperty.PropertyType)))
                .ToArray();

            var whereClause = parameters.Length.Select(i => "@p" + i).ToArray().Join(",");

            var context = ((IObjectContextAdapter)db).ObjectContext;
            var contextType = context.MetadataWorkspace.GetEntityContainer(context.DefaultContainerName, DataSpace.CSpace);
            var entitySetType = contextType.BaseEntitySets.Where(c => c.ElementType.Name == entityType.Name).FirstOrDefault();

            string tableName = entitySetType.Name;

            var sql = string.Format("delete from {0} where {1} in ({2})", tableName, keyProperty.Name, whereClause);
            db.Database.ExecuteSqlCommand(sql, parameters);
            db.SaveChanges();

            return this.Success();
        }

        #region 单个实体的CRUD

        protected T FindEntity<T>(string id = null) {
            return FindEntity(typeof(T), id);
        }

        protected dynamic FindEntity(Type entityType, string id = null) {
            object[] keyValues;
            if (!string.IsNullOrEmpty(id)) {
                keyValues = GetKeyValues(entityType, id);
            } else {
                object entityInstance = GetInstanceByType(entityType);
                TryUpdateModel(entityType, entityInstance);
                keyValues = GetKeyValues(entityType, entityInstance);
            }

            object model = db.Set(entityType).Find(keyValues);

            return model;
        }

        protected T AddEntity<T>(Action<dynamic> handleBeforeSubmit = null) {
            return AddEntity(typeof(T), handleBeforeSubmit);
        }

        protected dynamic AddEntity(Type entityType, Action<dynamic> handleBeforeSubmit = null) {
            object model = GetInstanceByType(entityType);

            var keyProperties = GetKeyProperties(entityType);
            TryUpdateModel(entityType, model);

            foreach (var keyProperty in keyProperties) {
                if (model.GetValue(keyProperty.Name) == null) {
                    //如果主键未赋值，则框架自动赋予guid
                    switch (keyProperty.PropertyType.FullName) {
                        case "System.Guid":
                            model.SetValue(keyProperty.Name, Guid.NewGuid());
                            //keyProperty.SetValue(model, Guid.NewGuid(), null););
                            break;
                        case "System.String":
                            model.SetValue(keyProperty.Name, Guid.NewGuid().ToString());
                            //keyProperty.SetValue(model, Guid.NewGuid().ToString(), null);
                            break;
                    }
                }
            }

            if (handleBeforeSubmit != null) {
                handleBeforeSubmit(model);
            }

            db.Set(model.GetType()).Add(model);
            db.SaveChanges();

            return model;
        }

        protected T UpdateEntity<T>(Action<dynamic> handleBeforeSubmit = null) {
            return UpdateEntity(typeof(T), handleBeforeSubmit);
        }

        protected dynamic UpdateEntity(Type entityType, Action<dynamic> handleBeforeSubmit = null) {
            object model = GetInstanceByType(entityType);

            TryUpdateModel(entityType, model);

            SetValue(model, "UpdateTime", DateTime.Now);
            SetValue(model, "UpdateUserId", this.UserID);

            if (handleBeforeSubmit != null) {
                handleBeforeSubmit(model);
            }

            db.Entry(model).State = EntityState.Modified;
            db.SaveChanges();

            return model;
        }

        protected T DeleteEntity<T>() {
            return DeleteEntity(typeof(T));
        }

        protected dynamic DeleteEntity(Type entityType) {
            object model = GetInstanceByType(entityType);
            TryUpdateModel(entityType, model);
            db.Entry(model).State = EntityState.Deleted;
            db.SaveChanges();
            return model;
        }

        #endregion

        #region 辅助方法

        private object[] GetKeyValues(Type entityType, object instance) {
            var keyProperties = GetKeyProperties(entityType);

            return keyProperties.Select(p => p.GetValue(instance, null)).ToArray();
        }

        private object[] GetKeyValues(Type entityType, string id) {
            var keyProperty = GetKeyProperties(entityType).FirstOrDefault();


            object key = id;
            if (id.GetType() != keyProperty.PropertyType) {
                key = Convert.ChangeType(id, keyProperty.PropertyType);
            }

            return new object[] { key };
        }

        private IEnumerable<PropertyInfo> GetKeyProperties(Type entityType) {
            var context = ((IObjectContextAdapter)db).ObjectContext;
            var tableType = context.MetadataWorkspace.GetItem<EntityType>(entityType.FullName, true, DataSpace.OSpace);
            var keyPropertyType = tableType.MetadataProperties.Where(c => c.Name == "KeyMembers").FirstOrDefault();

            if (keyPropertyType != null) {
                var keyProppertyNames = (IEnumerable<EdmMember>)keyPropertyType.Value;
                return keyProppertyNames.Select(p => entityType.GetProperty(p.Name));
            }

            return Enumerable.Empty<PropertyInfo>();
        }

        private Type GetEntityType(string name) {
            name.ThrowIfNullOrEmpty("name");

            Type entityType = EntityWatcher.GetEntityType(name);

            if (entityType == null) {
                throw new InvalidOperationException("Invalid entity type: " + name);
            }

            return entityType;
        }

        private static object GetInstanceByType(Type type) {
            return Activator.CreateInstance(type);
        }

        private static bool SetValue(object obj, string propertyName, object value) {
            obj.ThrowIfNull("obj");

            Type entityType = obj.GetType();

            var prop = entityType.GetProperty(propertyName);

            if (prop != null) {
                prop.SetValue(obj, value, null);

                return true;
            }

            return false;
        }

        #endregion

        #endregion

        #region Menu
        public ActionResult Menu() {
            var root = XElement.Load(this.Request.MapPath("~/content/sitemap/Web.sitemap"));
            root = root.Elements().First();

            var retrieveUrl = Self.Fix<XElement>((f, n) => {
                string url = RetrieveUrl(n);
                n.SetAttributeValue("url", url);

                bool isCurrent = (string.Compare(url, this.Request.Path, true) == 0);
                n.SetAttributeValue("class", isCurrent ? "current" : string.Empty);

                if (n.HasElements) {
                    foreach (var x in n.Elements()) {
                        f(x);
                    }

                    //if(isCurrent = (string.Compare(url, this.Request.Path, true) == 0);
                    //n.SetAttributeValue("class", isCurrent ? "current" : string.Empty);
                }
            });

            retrieveUrl(root);

            return View(root);
        }

        private string RetrieveUrl(XElement x) {
            if (x.Attribute("url") != null) {
                return Convert.ToString(x.Attribute("url").Value);
            }

            if (x.Attribute("action") != null && x.Attribute("controller") != null) {
                return Url.Action(x.Attribute("action").Value, x.Attribute("controller").Value);
            }

            return null;
        }

        private static bool FilterByRole(XElement x, string[] userRoles) {
            string role = x.Attribute("role").Value;
            if (string.IsNullOrEmpty(role)) {
                return true;
            }

            string[] roles = role.Split('|');
            string allowRole = roles[0];
            string forbidRole = string.Empty;
            if (roles.Length > 1) {
                forbidRole = roles[1];
            }

            if (forbidRole.Split(',').Where(r => userRoles.Contains(r)).Count() > 0) {
                return false;
            }

            if (allowRole == "*" || allowRole.Split(',').Where(r => userRoles.Contains(r)).Count() > 0) {
                return true;
            }

            return false;
        }
        #endregion
    }

    public class DynamicXml : DynamicObject {
        private XElement _root = null;

        public DynamicXml(XElement el) {
            _root = el;
        }

        public DynamicXml(string path) {
            if (!File.Exists(path))
                throw new FileNotFoundException();
            _root = XElement.Load(path);
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result) {
            result = null;
            if (binder.Name == "Elements") {
                result = _root.Elements(binder.Name).Select(x => new DynamicXml(x));
                return true;
            }
            result = _root.Attribute(binder.Name);
            return true;
        }

    }
}
