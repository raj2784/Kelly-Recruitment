using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KellyRecruitment.Models
{
	public class SQLEmployeeRespository : IEmployeeRepository
	{
		private readonly AppDbContext db;
		private readonly ILogger<SQLEmployeeRespository> logger;

		public SQLEmployeeRespository(AppDbContext db,
			    ILogger<SQLEmployeeRespository>logger)
		{
			this.db = db;
			this.logger = logger;
		}
		public Employee Add(Employee employee)
		{
			db.Employees.Add(employee);
			db.SaveChanges();
			return employee;
		}

		public Employee Delete(int Id)
		{
			Employee employee = db.Employees.Find(Id);
			if (employee != null)
			{
				db.Employees.Remove(employee);
				db.SaveChanges();
			}
			return employee;
		}

		public IEnumerable<Employee> GetAllEmployee()
		{
			return db.Employees;
		}

		public Employee GetEmployee(int Id)
		{
			logger.LogTrace("Trace Log");
			logger.LogDebug("Debug Log");
			logger.LogInformation("Information Log");
			logger.LogWarning("Warning Log");
			logger.LogError("Error Log");
			logger.LogCritical("Critical Log");

			return db.Employees.Find(Id);
		}

		public Employee Update(Employee employeechange)
		{
			var employee = db.Employees.Attach(employeechange);
			employee.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
			db.SaveChanges();
			return (employeechange);
		}
		
	}
}
