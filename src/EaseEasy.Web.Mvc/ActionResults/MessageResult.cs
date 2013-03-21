using System.Collections.Generic;
using System.Reflection;
using System.Web.Mvc;

namespace EaseEasy.Web.Mvc {
    /// <summary>
    /// 用于向客户端返回JSON格式的消息
    /// </summary>
    public class MessageResult : JsonResult {
        private IDictionary<string, object> resultSet = new Dictionary<string, object>();

        public MessageResult(bool success = true, string message = null) {
            resultSet["success"] = success;
            resultSet["message"] = message;
        }

        public MessageResult(bool success, object result) {
            resultSet["success"] = success;
            if (result != null) {
                if (result.GetType() == typeof(string)) {
                    resultSet["message"] = result;
                }
                else
                {
                    foreach (var property in result.GetType().GetProperties())
                    {
                        if (property.CanRead)
                        {
                            resultSet[property.Name] = property.GetValue(result, null);
                        }
                    }
                }
            }
        }

        public override void ExecuteResult(ControllerContext context) {
            base.Data = resultSet;// new { success = this.Success, message = this.Message };
            base.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            base.ExecuteResult(context);
        }
    }
}
