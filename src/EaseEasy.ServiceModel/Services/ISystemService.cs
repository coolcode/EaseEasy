using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EaseEasy.Linq;

namespace EaseEasy.ServiceModel.Services {
	public interface IDictionaryService {
		IEnumerable<ValueText> ListValueText(string name);
		IEnumerable<ValueText> ListEnabledValueText(string name);
		IEnumerable<string> ListDictionaryGroups();
		IEnumerable<DictionaryItem> GetDictionaryItems(string name);
		string GetDictionaryText(string dictionaryName, string key);
		DictionaryItem GetDictionaryItem(int id);
		void SaveDictionaryItem(DictionaryItem item, string user);
		void DeleteDictionaryItem(int id);
		IQueryable<DictionaryItem> QueryDictionaryItem(IQueryBuilder<DictionaryItem> cond);
	}
}
