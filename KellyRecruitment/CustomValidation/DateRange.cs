using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace KellyRecruitment.ModelsView
{
	public class DateRange : RangeAttribute
	{
		public DateRange(string minimumValue) : 
			base(typeof(DateTime),minimumValue,DateTime.Now.ToShortDateString())
		{

		}
	}
}
