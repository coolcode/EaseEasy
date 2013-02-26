using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace CoolCode.ServiceModel.Services {
	public class VirtualView {
		[Key]
		public int Id { get; set; }

		[Required]
		[MaxLength(200), Column(TypeName = "nvarchar")]
		public string Path { get; set; }

		[Column(TypeName = "nvarchar(max)")]
		public string Html { get; set; }

		[MaxLength(50), Column(TypeName = "varchar")]
		public string UpdateUserId { get; set; }

		public DateTime? UpdateTime { get; set; }
	}
}
