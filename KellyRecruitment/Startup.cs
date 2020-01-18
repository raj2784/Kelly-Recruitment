using KellyRecruitment.Models;
using KellyRecruitment.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KellyRecruitment
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.Configure<CookiePolicyOptions>(options =>
			{
				// This lambda determines whether user consent for non-essential cookies is needed for a given request.
				options.CheckConsentNeeded = context => true;
				options.MinimumSameSitePolicy = SameSiteMode.None;
			});

			services.AddDbContextPool<AppDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DbConnection")));

			services.AddIdentity<ApplicationUser, IdentityRole>(options =>

			{
				//options.Password.RequiredLength = 8;
				//options.Password.RequiredUniqueChars = 1;
				//options.Password.RequireLowercase = true;
				//options.Password.RequireUppercase = true;
				//options.Password.RequireNonAlphanumeric = true;
				//options.Password.RequireDigit = true;

				//Account Lockout Options Max fail attempts
				options.Lockout.MaxFailedAccessAttempts = 5;
				//Account Lockout options Time span
				options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);


				//options.SignIn.RequireConfirmedEmail = false;
				options.SignIn.RequireConfirmedEmail = true;

				//cuctome Email Confirmation token provider
				options.Tokens.EmailConfirmationTokenProvider = "CustomEmailConfirmation";



			}).AddEntityFrameworkStores<AppDbContext>()
			  .AddDefaultTokenProviders()
			//cuctome Email Confirmation token provider
			.AddTokenProvider<CustomEmailConfirmationTokenProvider<ApplicationUser>>("CustomEmailConfirmation");


			// this code change lifespan for all tokens type (Defaultokenprovider tokens have 1 day timespan by default)
			services.Configure<DataProtectionTokenProviderOptions>(options => options.TokenLifespan = TimeSpan.FromHours(5));

			//Custome Email Confirmation token provider(this code change the timespan for only Email confirmation token provider onlu)
			services.Configure<CustomEmailConfirmationTokenProviderOptions>(options => options.TokenLifespan = TimeSpan.FromDays(3));


			//services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
			services.AddMvc(options =>
			{
				// for apply authorization globally to the application
				var policy = new AuthorizationPolicyBuilder()
								  .RequireAuthenticatedUser()
								  .Build();
				options.Filters.Add(new AuthorizeFilter(policy));

			}).AddXmlDataContractSerializerFormatters();

			services.AddAuthentication()
				.AddGoogle(options =>
				{
					options.ClientId = "893957090775-dga23cq2j2pq92bq56d6suefppdrgl8j.apps.googleusercontent.com";
					options.ClientSecret = "MUEnfgsoY0y_7Y0ovzm_dIO1";
				})

			   .AddFacebook(options =>
			   {
				   options.AppId = "637052307038583";
				   options.AppSecret = "b2199596d49d568f0ae06ab2d5d158cc";

			   });


			services.AddAuthorization(options =>

			{
				//custom Authorization requirement only allow user to manage other user but not for their own
				//options.AddPolicy("EditRolePolicy", policy => policy.AddRequirements(new ManageAdminRolesAndClaimsRequirement()));

				//Custom policy create code
				options.AddPolicy("EditRolePolicy", policy => policy.RequireAssertion(context =>
				 context.User.IsInRole("Admin") &&
				  context.User.HasClaim(claim => claim.Type == "Edit Role") /*&& claim.Value == "true")*/ ||
				  context.User.IsInRole("SuperAdmin")));

				// can create seperate policy for each claim 
				options.AddPolicy("DeleteRolePolicy", policy => policy.RequireClaim("Delete Role")); //("Delete Role", "true"));

				options.AddPolicy("CreateRolePolicy", policy => policy.RequireClaim("Create Role")); //("Create Role", "true"));

				//options.AddPolicy("EditRolePolicy", policy => policy.RequireClaim("Edit Role", "true")); //("Edit Role", "true"));


				//Claims Policy to satisfy user must have all three claims, or else can create seprate claims policy for different claims

				options.AddPolicy("ClaimsPolicy", policy => policy.RequireClaim("Delete Role") //("Delete Role", "true")
																  .RequireClaim("Create Role") //("Create Role", "true")
																  .RequireClaim("Edit Role")); //("Edit Role", "true"));
				//Roles Policy
				options.AddPolicy("RolesPolicy", policy => policy.RequireRole("SuperAdmin", "Admin"));

			});


			services.AddScoped<IEmployeeRepository, SQLEmployeeRespository>();

			services.AddSingleton<IAuthorizationHandler, CanEditOnlyOtherAdminRolesAndClaimsHandler>();

			services.AddSingleton<IAuthorizationHandler, SuperAdminHandler>();


		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				//app.UseStatusCodePages();
				//app.UseStatusCodePagesWithRedirects("/Error/{0}");
				app.UseStatusCodePagesWithReExecute("/Error/{0}");

				app.UseExceptionHandler("/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				//app.UseHsts();
			}

			app.UseHttpsRedirection();
			app.UseStaticFiles();
			app.UseAuthentication();
			app.UseCookiePolicy();
			app.UseMvcWithDefaultRoute();
			app.UseMvc(routes =>
			{
				routes.MapRoute(
					name: "default",
					template: "{controller=Home}/{action=Index}/{id?}");
			});
		}
	}
}
