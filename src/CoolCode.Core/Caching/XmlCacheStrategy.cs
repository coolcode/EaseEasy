using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Collections;

namespace CoolCode.Caching {
	public class XmlCacheStrategy : ICacheStrategy {
		private readonly ICacheStrategy _innerCache = new HashCacheStrategy();// 这里应用了Strategy模式。
		private XmlElement _rootMap;				//	动态构建的 Xml文档 的根结点
		private XmlDocument _doc = new XmlDocument();	// 构建 Xml文档

		public XmlCacheStrategy(ICacheStrategy cache)
			: this() {
			_innerCache = cache;
		}

		public XmlCacheStrategy() {
			// 创建文档根结点，用于映射 实际的数据存储(例如Hashtable) 和 Xml文档
			_rootMap = _doc.CreateElement("Cache");
			// 添加根结点
			_doc.AppendChild(_rootMap);
		}

		// 根据 XPath 获取对象
		// 先根据Xpath获得对象的Key，然后再根据Key获取实际对象
		public virtual object Get(string xpath) {
			object obj = null;
			xpath = PrepareXPath(xpath);
			XmlNode node = _rootMap.SelectSingleNode(xpath);

			if (node != null) {
				string key = node.Attributes["key"].Value;
				obj = _innerCache.Get(key);
			}

			return obj;
		}

		// 获取一组对象，此时xpath为一个组结点
		public virtual object[] GetList(string xpath) {
			xpath = PrepareXPath(xpath);

			XmlNode group = _rootMap.SelectSingleNode(xpath);

			// 获取该结点下的所有子结点(使用[@key]确保子结点一定包含key属性)
			XmlNodeList results = group.SelectNodes(xpath + "/*[@key]");

			ArrayList objects = new ArrayList();

			foreach (XmlNode result in results) {
				string key = result.Attributes["key"].Value;
				object obj = _innerCache.Get(key);
				objects.Add(obj);
			}

			return (object[])objects.ToArray(typeof(object));
		}


		// 添加对象，对象实际上还是添加到ICacheStrategy指定的存储位置，
		// 动态创建的 Xml 结点仅保存了对象的Id(key)，用于映射两者间的关系
		public virtual void Insert(string xpath, object obj) {
			// 获取 Xpath，例如 /Cache/BookStore/Book/Title
			xpath = PrepareXPath(xpath);

			int separator = xpath.LastIndexOf("/");

			// 获取组结点的层叠顺序 ，例如 /Cache/BookStore/Book
			string group = xpath.Substring(0, separator);

			// 获取叶结点名称，例如 Title
			string element = xpath.Substring(separator + 1);

			// 获取组结点
			XmlNode groupNode = _rootMap.SelectSingleNode(group);

			// 如果组结点不存在，创建之
			if (groupNode == null) {
				lock (this) {
					groupNode = CreateNode(group);
				}
			}

			// 创建一个唯一的 key ，用来映射 Xml 和对象的主键
			string key = Guid.NewGuid().ToString();
			// 创建一个新结点
			XmlElement objectElement = _rootMap.OwnerDocument.CreateElement(element);
			// 创建结点属性 key
			XmlAttribute objectAttribute = _rootMap.OwnerDocument.CreateAttribute("key");
			// 设置属性值为 刚才生成的 Guid
			objectAttribute.Value = key;
			// 将属性添加到结点
			objectElement.Attributes.Append(objectAttribute);
			// 将结点添加到 groupNode 下面(groupNode为Xpath的层次部分)
			groupNode.AppendChild(objectElement);
			// 将 key 和 对象添加到实际的存储位置，比如Hashtable
			_innerCache.Insert(key, obj);
		}


		// 根据 XPath 删除对象
		public virtual void Remove(string xpath) {
			xpath = PrepareXPath(xpath);
			XmlNode result = _rootMap.SelectSingleNode(xpath);

			if (result.HasChildNodes) {
				// 选择所有包含有key属性的的结点
				XmlNodeList nodeList = result.SelectNodes("descendant::*[@key]");
				foreach (XmlNode node in nodeList) {
					string key = node.Attributes["key"].Value;
					node.ParentNode.RemoveChild(node);
					_innerCache.Remove(key);
				}
			}
			else {
				string key = result.Attributes["key"].Value;
				result.ParentNode.RemoveChild(result);
				_innerCache.Remove(key);
			}
		}


		// 根据 XPath 创建一个结点
		private XmlNode CreateNode(string xpath) {
			string[] xpathArray = xpath.Split('/');
			string nodePath = "";

			// 父节点初始化
			XmlNode parentNode = _rootMap;

			// 逐层深入 XPath 各层级，如果结点不存在则创建。比如 /DvdStore/Dvd/NoOneLivesForever
			for (int i = 1; i < xpathArray.Length; i++) {
				XmlNode node = _rootMap.SelectSingleNode(nodePath + "/" + xpathArray[i]);

				if (node == null) {
					XmlElement newElement = _rootMap.OwnerDocument.CreateElement(xpathArray[i]);	// 创建结点
					parentNode.AppendChild(newElement);
				}

				// 创建新路径，更新父节点，进入下一级
				nodePath = nodePath + "/" + xpathArray[i];
				parentNode = _rootMap.SelectSingleNode(nodePath);
			}

			return parentNode;
		}

		// 构建 XPath，使其以 /Cache 为根结点，并清除多于的"/"字符
		private static string PrepareXPath(string xpath) {
			string[] xpathArray = xpath.Split('/');
			xpath = "/Cache";		// 这里的名称需与构造函数中创建的根结点名称对应
			foreach (string s in xpathArray) {
				if (s != "") {
					xpath += "/" + s;
				}
			}

			return xpath;
		}

		#region IEnumerable Members

		IEnumerator IEnumerable.GetEnumerator() {
			return _innerCache.GetEnumerator();
		}

		#endregion
	}
}
