using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using KellyRecruitment.Models;
using KellyRecruitment.ModelsView;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;

namespace KellyRecruitment.Controllers
{
	[Authorize]
	public class HomeController : Controller
	{
		private readonly IEmployeeRepository empRepository;
		private readonly IHostingEnvironment hostingEnvironment;
		private readonly AppDbContext db;
		private readonly ILogger<HomeController> logger;

		public HomeController(IEmployeeRepository employeeRepository,
							  IHostingEnvironment hostingEnvironment,
							  AppDbContext context,
							  ILogger<HomeController> logger)

		{
			empRepository = employeeRepository;
			this.hostingEnvironment = hostingEnvironment;
			db = context;
			this.logger = logger;
		}
		private string ProcessUploadedFile(CreateEmployeViewModel model)
		{
			string PhotoName = null;
			if (model.Photo != null)
			{
				string uploadsFolder = Path.Combine(hostingEnvironment.WebRootPath, "Images");
				PhotoName = DateTime.Now.ToString("yyyyMMddhhmmss") + "-" + model.Photo.FileName;
				//to save Uniqfile name should use time stemp or Guid method
				//PhotoName = Guid.NewGuid().ToString() + "-" + model.Photo.FileName;
				string filePath = Path.Combine(uploadsFolder, PhotoName);
				using (var fileStrean = new FileStream(filePath, FileMode.Create))
				{
					model.Photo.CopyTo(fileStrean);
				}
			}

			return PhotoName;
		}
		[HttpGet]
		public IActionResult Create()
		{
			return View();
		}
		[HttpPost]
		public IActionResult Create(CreateEmployeViewModel model)
		{
			if (ModelState.IsValid)
			{
				string PhotoName = ProcessUploadedFile(model);
				Employee newEmployee = new Employee
				{
					Name = model.Name,
					Email = model.Email,
					Dob = model.Dob,
					Gender = model.Gender,
					Department = model.Department,
					PhotoPath = PhotoName

				};
				empRepository.Add(newEmployee);

				return RedirectToAction("Details", new { id = newEmployee.Id });
			}
			return View();
		}

		[AllowAnonymous]
		public IActionResult Index()
		{

			var model = empRepository.GetAllEmployee();
			return View(model);

		}
		[AllowAnonymous]
		public IActionResult ListView()
		{

			var model = empRepository.GetAllEmployee();
			return View(model);

		}
		[HttpGet]
		public IActionResult Edit(int id)
		{
			Employee employee = empRepository.GetEmployee(id);
			EditEmployeeViewModel editEmployeeViewModel = new EditEmployeeViewModel
			{
				Id = employee.Id,
				Name = employee.Name,
				Email = employee.Email,
				Dob = employee.Dob,
				Gender = employee.Gender,
				Department = employee.Department,
				ExistingPhotoPath = employee.PhotoPath
			};

			return View(editEmployeeViewModel);
		}
		[HttpPost]
		public IActionResult Edit(EditEmployeeViewModel model)
		{
			if (ModelState.IsValid)
			{
				Employee employee = empRepository.GetEmployee(model.Id);

				employee.Name = model.Name;
				employee.Dob = model.Dob;
				employee.Gender = model.Gender;
				employee.Email = model.Email;
				employee.Department = model.Department;
				if (model.Photo != null)
				{
					if (model.ExistingPhotoPath != null)
					{
						string filePath = Path.Combine(hostingEnvironment.WebRootPath, "Images", model.ExistingPhotoPath);
						System.IO.File.Delete(filePath);
					}
					employee.PhotoPath = ProcessUploadedFile(model);
				}

				empRepository.Update(employee);
				return RedirectToAction("Index", "Home");
			}
			return View();
		}
		public IActionResult Details(int? Id)
		{
			//throw new Exception("error in detail view");
			logger.LogTrace("Trace Log");
			logger.LogDebug("Debug Log");
			logger.LogInformation("Information Log");
			logger.LogWarning("Warning Log");
			logger.LogError("Error Log");
			logger.LogCritical("Critical Log");

			var employee = empRepository.GetEmployee(Id.Value);

			if (employee == null)
			{
				Response.StatusCode = 404;
				return View("EmployeeNotFound", Id.Value);
			}

			return View(employee);
		}

		[HttpGet]
		public IActionResult Delete(int Id)
		{
			Employee employee = empRepository.GetEmployee(Id);
			if (employee != null)
			{
				var result = empRepository.Delete(employee.Id);

				return RedirectToAction("Index");
			}
			return RedirectToAction("Index");


		}
		public IActionResult Privacy()
		{
			return View();
		}
		public IActionResult Home()
		{
			return View();
		}
		[HttpGet]
		[AllowAnonymous]
		public IActionResult ContactUs()
		{
			return View();
		}
		[HttpPost]
		[AllowAnonymous]
		public IActionResult ContactUs(ContactUs model)
		{
			if (ModelState.IsValid)
			{
				ContactUs contactUs = new ContactUs
				{
					Name = model.Name,
					Email = model.Email,
					Mobile = model.Mobile,
					Message = model.Message
				};
				db.ContactUstbl.Add(contactUs);
				db.SaveChanges();
				return View("InquiryConfirmationView");

			}
			return View();
		}


		//[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		//public IActionResult Error()
		//{
		//	return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		//}
	}
}
