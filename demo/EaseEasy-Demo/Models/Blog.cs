using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace EaseEasy_Demo.Models {
	public class Blog {
		public int Id { get; set; } 
		public string Title { get; set; } 
		public string Author { get; set; }
	}
}