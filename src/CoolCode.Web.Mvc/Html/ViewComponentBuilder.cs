using System;
using System.Web;
using System.Web.Mvc;
using CoolCode.Web.Mvc.UI;

namespace CoolCode.Web.Mvc.Html {
	public static class ViewComponentBuilderExtensions {
		#region Element

		public static TBuilder Element<TComponent, TBuilder>(this HtmlHelper helper)
			where TComponent : ViewComponent, new()
			where TBuilder : ViewComponentBuilder<TComponent, TBuilder>, IHtmlString {
			var component = new TComponent();
			var builder = Activator.CreateInstance(typeof(TBuilder), component, helper.ViewContext, helper.ViewDataContainer);
			return (TBuilder)builder;
		}

		#endregion

		public static TBuilder Width<TBuilder, TComponent>(this IFluentViewComponentBuilder<TComponent, TBuilder> builder, int value)
			where TComponent : ViewComponent
			where TBuilder : IFluentViewComponentBuilder<TComponent, TBuilder> {
			builder.Component.Width = value;
			return (TBuilder)builder;
		}
	}
}
