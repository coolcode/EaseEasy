using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoolCode.ServiceModel.Services.Implement {
	public class DefaultEntityWatcher:IEntityWatcher {
		#region IEntityWatcher Members

		public virtual Type GetEntityType(string entityName) {
			return Type.GetType("CoolCode.ServiceModel.Services." + entityName + ",CoolCode.ServiceModel");
		}

		#endregion
	}
}
