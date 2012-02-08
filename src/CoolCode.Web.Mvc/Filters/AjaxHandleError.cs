using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace CoolCode.Web.Mvc {
	/// <summary>
	/// 当Ajax请求发生错误时，返回json格式的数据：{success = false, message = 异常描述, detail = 异常堆栈信息}
	/// </summary> 
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
	public class AjaxHandleErrorAttribute : HandleErrorAttribute {
		public override void OnException(ExceptionContext filterContext) {
			base.OnException(filterContext);
			if (filterContext.HttpContext.Request.IsAjaxRequest()) {
				var ex = filterContext.Exception;
				var serializer = new JavaScriptSerializer();
				var error = serializer.Serialize(new {
					success = false,
					message = ex.Message,
					detail = ex.StackTrace.Replace("\r\n", "<br/>").Replace("\n", "<br/>")
				});

				filterContext.HttpContext.Response.Clear();
				filterContext.HttpContext.Response.Write(error);
				filterContext.HttpContext.Response.End();
			}
		}

	}

}
