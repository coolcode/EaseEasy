using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using CoolCode.Linq;

namespace CoolCode.ServiceModel.Services.Implement {
	public class DictionaryService : ServiceBase, IDictionaryService {
		#region Data Access Objects

		private DbSet<DictionaryItem> _dictionaryItemDao;
		public DbSet<DictionaryItem> DictionaryItemDao {
			get {
				return _dictionaryItemDao ?? (_dictionaryItemDao = db.Set<DictionaryItem>());
			}
			set {
				_dictionaryItemDao = value;
			}
		}

		#endregion

		#region ISystemService Members

		public IEnumerable<DictionaryItem> GetDictionaryItems(string name) {
			var query = from c in this.DictionaryItemDao
						where c.Name == name
						orderby c.SortIndex
						select c;
			return query;
		}

		public IEnumerable<ValueText> ListValueText(string name) {
			var query = from c in GetDictionaryItems(name)
						select new ValueText {
							Value = c.Value,
							Text = c.Text
						};
			return query;
		}

		public IEnumerable<ValueText> ListEnabledValueText(string name) {
			var query = from c in GetDictionaryItems(name)
						where c.Enabled
						select new ValueText {
							Value = c.Value,
							Text = c.Text
						};
			return query;
		}

		public IEnumerable<string> ListDictionaryGroups() {
			var query = (from c in this.DictionaryItemDao 
						 select c.Name).Distinct();
			return query.ToList();
		}

		public DictionaryItem GetDictionaryItem(int id) {
			return this.DictionaryItemDao.Find(id);
		}

		public string GetDictionaryText(string dictionaryName, string key) {
			return this.DictionaryItemDao
				.Where(c => c.Name == dictionaryName && c.Value == key)
				.Select(c => c.Text)
				.FirstOrDefault();
		}

		public void SaveDictionaryItem(DictionaryItem item, string user) {
			var entity = GetDictionaryItem(item.Id);
			if (entity != null) {
				item.SortIndex = entity.SortIndex;
				db.Entry(item).State = EntityState.Modified;
			}
			else {
				item.SortIndex = GetDictionaryItems(item.Name).Count() + 1;
				this.DictionaryItemDao.Add(item);
			}
			db.SaveChanges();
		}

		public void DeleteDictionaryItem(int id) { 
			var item = new DictionaryItem  {Id = id};
			this.db.Entry(item).State = EntityState.Deleted;
			db.SaveChanges();
		}

		public IQueryable<DictionaryItem> QueryDictionaryItem(IQueryBuilder<DictionaryItem> cond) {
			return this.DictionaryItemDao.Where(cond.Expression);
		}
		#endregion
	}
}
