using System.IO;

namespace EaseEasy.Web.Mvc.UI {
	/// <summary>
	/// ��ȾGridView�ӿ�
	/// </summary>
	public interface IGridViewRenderer<T> {
		void Render(IGridView<T> gridModel, TextWriter output);
	}
}