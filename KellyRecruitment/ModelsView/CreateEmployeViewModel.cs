using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using KellyRecruitment.Models;
using Microsoft.AspNetCore.Http;

namespace KellyRecruitment.ModelsView
{
	public class CreateEmployeViewModel
	{

		[Required]
		[RegularExpression(@"^(([A-za-z]+[\s]{1}[A-za-z]+)|([A-Za-z]+))$", ErrorMessage = "Only Alphabets are allowed")]
		public string Name { get; set; }
		[DateRange("01/01/2000",ErrorMessage = "Dob should be after 01/01/2000 to till date only")]
		[DataType(DataType.DateTime)]
		//[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
		public DateTime Dob { get; set; }

		[Required]
		public string Gender { get; set; }

		[EmailAddress]
		[Required]
		public string Email { get; set; }
		[Required]
		public Dept Department { get; set; }

		public IFormFile Photo { get; set; }

	}
}

