using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KellyRecruitment.Models;

namespace KellyRecruitment.Models
{
	public interface IEmployeeRepository
	{
		Employee GetEmployee(int Id);
		IEnumerable<Employee>GetAllEmployee();
		Employee Add(Employee employee);
		Employee Update(Employee employeechange);
		Employee Delete(int Id);

	}
}
