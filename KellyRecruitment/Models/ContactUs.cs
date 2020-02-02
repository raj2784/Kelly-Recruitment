using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace KellyRecruitment.Models
{
	public class ContactUs
	{
		[Key]
		public int Id { get; set; }
		[Required]
		[Column(TypeName = "nvarchar(50)")]
		public string Name { get; set; }
		[Required]
		[EmailAddress]
		[Column(TypeName = "nvarchar(50)")]
		public string Email { get; set; }
		[Required]
		public string Mobile { get; set; }
		[Required]
		[Column(TypeName = "nvarchar(200)")]
		[MaxLength(1024), MinLength(5)]
		public string Message { get; set; }
	}
}
