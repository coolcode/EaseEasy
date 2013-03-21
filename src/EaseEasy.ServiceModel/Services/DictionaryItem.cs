using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace EaseEasy.ServiceModel.Services {
	public class DictionaryItem {
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		[Required]
		[MaxLength(50), Column(TypeName = "nvarchar")]
		public string Name { get; set; }

		[Required]
		[MaxLength(50), Column(TypeName = "varchar")]
		public string Value { get; set; }

		[MaxLength(255), Column(TypeName = "nvarchar")]
		public string Text { get; set; }

		public int SortIndex { get; set; }
		public bool Enabled { get; set; }
		public bool IsInner { get; set; }
	}
}
