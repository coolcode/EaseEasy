using System;
using System.Web.Mvc;

namespace CoolCode.Web.Mvc.UI {
	public abstract class TemplateViewBuilder<TComponent> : TemplateViewBuilder<TComponent, TemplateViewBuilder<TComponent>>
		where TComponent : TemplateViewComponent {
		public TemplateViewBuilder(TComponent component, ViewContext viewContext, IViewDataContainer viewContainer) : base(component, viewContext, viewContainer) { }
	}

	public abstract class TemplateViewBuilder<TComponent, TBuilder> : ViewComponentBuilder<TComponent, TBuilder>
		where TComponent : TemplateViewComponent
		where TBuilder : TemplateViewBuilder<TComponent, TBuilder> {
		public TemplateViewBuilder(TComponent component, ViewContext viewContext, IViewDataContainer viewContainer) : base(component, viewContext, viewContainer) { }

		public TBuilder Template(Action value) {
			Component.Template.Content = value;
			return this as TBuilder;
		}

		public TBuilder Template(Func<object, object> value) {
			this.Component.Template.InlineContent = value;
			return this as TBuilder;
		}
	}

	public abstract class TemplateViewBuilder<TModel, TComponent, TBuilder> : ViewComponentBuilder<TComponent, TBuilder>
		where TModel : class
		where TComponent : TemplateViewComponent<TModel>
		where TBuilder : TemplateViewBuilder<TModel, TComponent, TBuilder> {
		public TemplateViewBuilder(TComponent component, ViewContext viewContext, IViewDataContainer viewContainer) : base(component, viewContext, viewContainer) { }

		public TBuilder Template(Func<TModel, object> value) {
			this.Component.Template.InlineContent = value;
			return this as TBuilder;
		}

		public virtual TBuilder Template(Action<TModel> value) {
			this.Component.Template.Content = value;
			return this as TBuilder;
		}
	}
}
