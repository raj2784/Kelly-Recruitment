using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KellyRecruitment.ModelsView
{
	public class EditEmployeeViewModel : CreateEmployeViewModel
	{
		public int Id { get; set; }
		public string ExistingPhotoPath { get; set; }
	}
}
