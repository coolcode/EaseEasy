using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoolCode.Caching {
	public static class CacheHelper {
		public static TData Get<TData>(Func<object> cacheGetter, Func<TData> sourceGetter, Action<TData> cacheSetter) {
			var dataFromCache = cacheGetter();
			if ( dataFromCache != null) {
				return (TData)dataFromCache  ;
			}

			var dataFromCustom = sourceGetter();
			cacheSetter(dataFromCustom);

			return dataFromCustom;
		}
	}
	/* Sample: 
	 return CacheHelper.Get(
		() => // cache getter
		{
			return cache.Get(cacheKey);  
		},
		() => // source getter
		{
			return new UserService().GetFriends(userId);
		},
		(data) => // cache setter
		{
			cache.Set(cacheKey, data);
		});
	 */
}
