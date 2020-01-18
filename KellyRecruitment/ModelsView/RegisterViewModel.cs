using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace KellyRecruitment.ModelsView
{
	public class RegisterViewModel
	{
		[Required]
		[EmailAddress]
		[Remote(action: "IsEmailInUse", controller: "Account")]
		public string Email { get; set; }

		[Required]
		[DataType(DataType.Password)]
		public string Password { get; set; }

		[Required]
		[DataType(DataType.Password)]
		[Compare("Password", ErrorMessage = "Confirm Password should be the same as Password")]
		[Display(Name = "Confirm Password")]
		public string ConfrimPassword { get; set; }
		public string City { get; set; }
	}
}
