namespace EaseEasy.Web.Mvc.UI {
	public interface IViewComponentBuilder<out TComponent> where TComponent : ViewComponent {
		TComponent Component { get; }
	}
}
