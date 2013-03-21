using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace EaseEasy.ServiceModel.Services {
	public class DefaultDbContext : DbContext {
		public DbSet<DictionaryItem> DictionaryItems { get; set; }
		public DbSet<VirtualView> VirtualViews { get; set; }

		public DefaultDbContext() : base("DefaultContext") { }
		 
	}
}
