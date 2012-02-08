namespace CoolCode.Web.Mvc.UI {
	/// <summary>
	/// ViewComponentBuilder fluent interface
	/// </summary>
	/// <remarks>
	/// 在下列情况下才使用ViewComponentBuilder fluent interface:
	/// 1. 子类和基类都实现链式编程；
	/// 2. 基类的方法返回的基类类型，子类实例调用后由于返回基类类型，而无法显式再调用子类方法；
	/// 3. 希望对基类的扩展可以应用于所有子类，并且子类实例调用该扩展方法后仍然保持流畅接口；
	/// 4. 通过对接口扩展，而非继承。
	/// </remarks>
	/// <typeparam name="TComponent"></typeparam>
	/// <typeparam name="TBuilder"></typeparam>
	public interface IFluentViewComponentBuilder<out TComponent, out TBuilder> : IViewComponentBuilder<TComponent>, IFluentInterface
		where TBuilder : IFluentViewComponentBuilder<TComponent, TBuilder>
		where TComponent : ViewComponent {

	}
}
