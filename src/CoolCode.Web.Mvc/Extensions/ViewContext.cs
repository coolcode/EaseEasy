using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;

namespace CoolCode.Web.Mvc {
    public static class ViewContextExtensions {
        public static string Url(this ViewContext viewContext, IDictionary<string, object> routeValues = null, bool keepQueryString = true, bool keepForm = true, string[] excludedKeys = null) {
            var newRouteValues = new RouteValueDictionary();

            //忽略Key是guid的情况
            Guid g;
            foreach (var keyValuePair in viewContext.RouteData.Values.Where(c => !Guid.TryParse(c.Key, out g))) {
                newRouteValues.Add(keyValuePair.Key, keyValuePair.Value);
            }

            //保留查询字符到RouteValues
            if (keepQueryString) {
                var queryString = viewContext.HttpContext.Request.QueryString;
                foreach (string key in queryString.Keys) {
                    if (!string.IsNullOrWhiteSpace(key)) {
                        newRouteValues[key] = queryString[key];
                    }
                }
            }

            //保留表单值到RouteValues
            if (keepForm) {
                var form = viewContext.HttpContext.Request.Form;
                foreach (string key in form.Keys) {
                    newRouteValues[key] = form[key];
                }
            }

            //自定义参数
            if (routeValues != null) {
                routeValues.ForEach(v => newRouteValues[v.Key] = v.Value);
            }

            //筛选
            if (excludedKeys != null) {
                foreach (var key in excludedKeys.Where(newRouteValues.ContainsKey)) {
                    newRouteValues.Remove(key);
                }
            }

            UrlHelper urlHelper = new UrlHelper(viewContext.RequestContext);
            string url = urlHelper.Action(viewContext.RouteData.Values["action"].ToString(), newRouteValues);

            return url;
        }
    }
}
