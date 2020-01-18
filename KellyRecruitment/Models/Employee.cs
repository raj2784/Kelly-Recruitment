using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace KellyRecruitment.Models
{
	public class Employee
	{
		[Key]
		public int Id { get; set; }

		[Required]
		[Column(TypeName = "nvarchar(50)")]
		public string Name { get; set; }

		[DataType(DataType.Date)]
		[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
		public DateTime Dob { get; set; }
		[Required]
		public string Gender { get; set; }

		[Column(TypeName = "nvarchar(50)")]
		[EmailAddress]
		public string Email { get; set; }

		public Dept Department { get; set; }

		public string PhotoPath { get; set; }
	}

}
