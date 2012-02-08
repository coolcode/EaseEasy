using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace CoolCode.Data.Entity {
	/// <summary>
	/// TODO:EF DbSet扩展方法
	/// </summary>
	public static class DbSetExtensions {
		public static void Delete<TEntity>(this IDbSet<TEntity> dbSet, params  object[] keyValues) where TEntity : class {
			  
		}

	}
}
