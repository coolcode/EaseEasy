using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace EaseEasy.Web.Mvc  {
	public static class ModelStateExtensions {
		public static void AddModelErrors(this ModelStateDictionary modelState, IEnumerable<RuleViolation> errors) {
			foreach (RuleViolation issue in errors) {
				modelState.AddModelError(issue.PropertyName, issue.ErrorMessage);
			}
		}
	}
}
