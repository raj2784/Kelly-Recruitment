using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KellyRecruitment.Models
{
	public class AppDbContext : IdentityDbContext<ApplicationUser>
	{
		public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)

		{

		}
		public DbSet<Employee> Employees { get; set; }

		public DbSet<ContactUs> ContactUstbl { get; set; }

		protected override void OnModelCreating(ModelBuilder modelbuilder)
		{
			base.OnModelCreating(modelbuilder);

			foreach (var foreignkey in modelbuilder.Model.GetEntityTypes()
				.SelectMany(e => e.GetForeignKeys()))
			{
				foreignkey.DeleteBehavior = DeleteBehavior.Restrict;
			}
		}
	}
}
