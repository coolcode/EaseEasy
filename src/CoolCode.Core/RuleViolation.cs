using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoolCode {
	public class RuleViolation {
		public string ErrorMessage { get; private set; }
		public string PropertyName { get; private set; }

		public RuleViolation(string errorMessage, string propertyName = null) {
			ErrorMessage = errorMessage;
			PropertyName = propertyName;
		}
	}
}
