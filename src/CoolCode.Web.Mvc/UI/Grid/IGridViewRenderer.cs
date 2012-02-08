using System.IO;

namespace CoolCode.Web.Mvc.UI {
	/// <summary>
	/// äÖÈ¾GridView½Ó¿Ú
	/// </summary>
	public interface IGridViewRenderer<T> {
		void Render(IGridView<T> gridModel, TextWriter output);
	}
}