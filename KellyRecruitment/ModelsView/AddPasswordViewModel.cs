using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace KellyRecruitment.ModelsView
{
	public class AddPasswordViewModel
	{
		[Required]
		[DataType(DataType.Password)]
		[Display(Name = "New Password")]
		public string NewPassword { get; set; }
		[Required]
		[DataType(DataType.Password)]
		[Display(Name = "Confirm Password")]
		[Compare("NewPassword",ErrorMessage = "Confirm password must be matched with New Password!")]
		public string ConfirmPassword { get; set; }

	}
}
