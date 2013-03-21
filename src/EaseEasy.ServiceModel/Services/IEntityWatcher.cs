using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EaseEasy.ServiceModel.Services {
	public interface IEntityWatcher {
		Type GetEntityType(string entityName);
	}
}
