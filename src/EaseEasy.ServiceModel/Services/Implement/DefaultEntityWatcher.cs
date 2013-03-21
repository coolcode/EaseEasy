using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EaseEasy.ServiceModel.Services.Implement {
	public class DefaultEntityWatcher:IEntityWatcher {
		#region IEntityWatcher Members

		public virtual Type GetEntityType(string entityName) {
			return Type.GetType("EaseEasy.ServiceModel.Services." + entityName + ",EaseEasy.ServiceModel");
		}

		#endregion
	}
}
