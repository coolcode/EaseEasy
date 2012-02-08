using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoolCode.Caching {
	/// <summary>
	/// 缓存策略接口
	/// </summary>
	public interface ICacheStrategy : IEnumerable {
		void Insert(string key, object obj);
		object Get(string key);
		void Remove(string key); 
	}
}
