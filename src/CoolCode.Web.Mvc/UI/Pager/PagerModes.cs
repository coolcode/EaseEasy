using System;

namespace CoolCode.Web.Mvc.UI {
	/// <summary>
	/// 分页类型
	/// </summary>
	[Flags]
	public enum PagerModes {
		Numeric = 0x1,
		NextPrevious = 0x2,
		FirstLast = 0x4,
		All = 0x7
	}
}
