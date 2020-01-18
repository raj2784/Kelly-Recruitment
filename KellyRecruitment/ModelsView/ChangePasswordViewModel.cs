using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace KellyRecruitment.ModelsView
{
	public class ChangePasswordViewModel
	{
		[Required]
		[DataType(DataType.Password)]
		[Display(Name = "Current Password")]
		public string CurrentPassword { get; set; }

		[Required]
		[DataType(DataType.Password)]
		[Display(Name = "New Password")]
		public string NewPassword { get; set; }

		[Required]
		[DataType(DataType.Password)]
		[Display(Name = "Confrim Password")]
		[Compare("NewPassword", ErrorMessage = "Confirm Password must be match with new password")]
		public string ConfirmPassword { get; set; }
	}
}
