using System;
using System.Web.Mvc;
using EaseEasy.ServiceModel.Logging;
using EaseEasy.ServiceModel.Security;
using EaseEasy.Web.Mvc;

namespace EaseEasy.ServiceModel.Mvc {
	public class ControllerBase : Controller {

		public string UserID {
			get { return User.Identity.Name; }
		}

		public T RetrieveModel<T>() where T : class, new() {
			T model = new T();
			TryUpdateModel<T>(model);
			return model;
		}

		protected static readonly IDependencyResolver CurrentResolver = DependencyResolver.Current;
		#region Services

		private IMembership _MembershipService;
		public IMembership MembershipService {
			get {
				if (_MembershipService == null) {
					_MembershipService = CurrentResolver.GetService<IMembership>();
				}
				return _MembershipService;
			}
		}

		private IFormsAuthentication _FormsAuthenticationService;
		public IFormsAuthentication FormsAuthenticationService {
			get {
				if (_FormsAuthenticationService == null) {
					_FormsAuthenticationService = CurrentResolver.GetService<IFormsAuthentication>();
				}
				return _FormsAuthenticationService;
			}
		}

		#endregion

		protected static readonly ILogger Logger = LogManager.GetLogger(typeof(ControllerBase));

		public ControllerBase() {
			ViewData["starttime"] = DateTime.Now;
			//LogManager.Config();  
		}

		protected override void OnException(ExceptionContext filterContext) {
			string error = string.Format("系统出现未处理错误:\r\n\tUser:{0},Controller:{1}, Action: {2}, RouteData:{3}\r\n\t",
				filterContext.HttpContext.User.Identity.Name,
				filterContext.Controller,
				filterContext.RouteData.Values["action"],
				filterContext.RouteData.ToQueryString()
			);
			Logger.Error(error, filterContext.Exception);

			base.OnException(filterContext);
		}

		public ActionResult RedirectToError(string msg) {
			return RedirectToAction("Error", new { id = msg });
		}

		public ActionResult RedirectToNotFound() {
			return RedirectToAction("NotFound");
		}

		public ActionResult NotFound() {
			return View();
		}

		public ActionResult Error(string id) {
			ViewData["Message"] = id;
			return View();
		}

	}


}
