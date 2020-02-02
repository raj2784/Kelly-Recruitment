using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace KellyRecruitment.ModelsView
{
	public class CreateContactUsViewModel
	{

		[Required]
		[MaxLength(20)]
		[MinLength(3)]
		public string Name { get; set; }
		[Required]
		[EmailAddress]
		public string Email { get; set; }
		[Required]
		public string Mobile { get; set; }
		[Required]
		[MaxLength(1024)]
		public string Message { get; set; }
	}
}
