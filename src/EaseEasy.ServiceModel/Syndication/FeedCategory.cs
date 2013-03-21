using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EaseEasy.Web.Mvc.Syndication {
	public class FeedCategory : IEqualityComparer<FeedCategory> {
		public string ID { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }

		#region IEqualityComparer<FeedCategory> Members

		public bool Equals(FeedCategory x, FeedCategory y) {
			if (string.IsNullOrEmpty(x.ID) || string.IsNullOrEmpty(y.ID)) {
				return false;
			}

			return x.ID == y.ID;
		}

		public int GetHashCode(FeedCategory obj) {
			if (string.IsNullOrEmpty(obj.ID)) {
				return obj.GetHashCode();
			}

			return obj.ID.GetHashCode();
		}

		#endregion
	}
}
