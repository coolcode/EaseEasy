using System;
using System.Collections.Generic;
using System.Text;

namespace EaseEasy {
	/// <summary>
	/// Singleton泛型类
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public sealed class Singleton<T> where T : new() {
		private readonly static T instance = new T();

		/// <summary>
		/// 获取实例
		/// </summary> 
		public static T GetInstance() {
			return instance;
		}
	}
}