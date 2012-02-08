using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Web.Security;

namespace CoolCode.Web.Mvc.DataAnnotations {
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
	public sealed class PropertiesMustMatchAttribute : ValidationAttribute {
		private const string DefaultErrorMessage = "\"{0}\" 和 \"{1}\" 不匹配";

		private readonly object _typeId = new object();

		public PropertiesMustMatchAttribute(string originalProperty, string confirmProperty)
			: base(DefaultErrorMessage) {
			OriginalProperty = originalProperty;
			ConfirmProperty = confirmProperty;
		}

		public string ConfirmProperty {
			get;
			private set;
		}

		public string OriginalProperty {
			get;
			private set;
		}

		public override object TypeId {
			get {
				return _typeId;
			}
		}

		public override string FormatErrorMessage(string name) {
			return String.Format(CultureInfo.CurrentUICulture, ErrorMessageString,
				OriginalProperty, ConfirmProperty);
		}

		public override bool IsValid(object value) {
			PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(value);
			object originalValue = properties.Find(OriginalProperty, true /* ignoreCase */).GetValue(value);
			object confirmValue = properties.Find(ConfirmProperty, true /* ignoreCase */).GetValue(value);
			return Object.Equals(originalValue, confirmValue);
		}
	}

	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public sealed class ValidatePasswordLengthAttribute : ValidationAttribute {
		private const string _defaultErrorMessage = "\"{0}\" 至少是 {1} 个字符";

		private readonly int _minCharacters = Membership.Provider.MinRequiredPasswordLength;

		public ValidatePasswordLengthAttribute()
			: base(_defaultErrorMessage) {
		}

		public override string FormatErrorMessage(string name) {
			return String.Format(CultureInfo.CurrentUICulture, ErrorMessageString,
				name, _minCharacters);
		}

		public override bool IsValid(object value) {
			string valueAsString = value as string;
			return (valueAsString != null && valueAsString.Length >= _minCharacters);
		}
	}

}
