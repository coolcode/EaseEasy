using System.Web.Mvc;

namespace CoolCode.Web.Mvc
{
	/// <summary>
	/// 用于向客户端返回JSON格式的消息
	/// </summary>
	public class MessageResult : JsonResult
	{
		public bool Success { get; set; }
		public string Message { get; set; }
 
		public MessageResult(bool success = true, string message = null)
		{
			this.Success = success;
			this.Message = message;
		}

		public override void ExecuteResult(ControllerContext context)
		{
			base.Data = new { success = this.Success, message = this.Message };
			base.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
			base.ExecuteResult(context);
		}
	}
}
