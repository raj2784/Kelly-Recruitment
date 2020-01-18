using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace KellyRecruitment.ModelsView
{
	public class ResetPasswordViewModel
	{
		[Required]
		[EmailAddress]
		public string Email { get; set; }
		[Required]
		[DataType(DataType.Password)]
		[Display(Name = "New Password")]
		public string NewPassword { get; set; }
		[Required]
		[DataType(DataType.Password)]
		[Display(Name ="Confrim Password")]
		[Compare("NewPassword", ErrorMessage = "Confirm Password must be match with new password")]
		public string ConfirmPassword { get; set; }
		[Required]
		public string Token { get; set; }
	}
}
