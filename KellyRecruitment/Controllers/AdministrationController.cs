using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using KellyRecruitment.ModelsView;
using KellyRecruitment.Models;
using Microsoft.AspNetCore.Authorization;
using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace KellyRecruitment.Controllers
{

	[Authorize(Policy = "RolesPolicy")]
	//user must me memeber of either of this role 
	//[Authorize(Roles = "SuperAdmin, Admin")]
	//user must me member of both this role
	//[Authorize(Roles = "SuperAdmin")]
	//[Authorize(Roles = "Admin")]
	public class AdministrationController : Controller
	{
		private readonly RoleManager<IdentityRole> roleManager;
		private readonly UserManager<ApplicationUser> userManager;
		private readonly ILogger<AdministrationController> logger;

		public AdministrationController(RoleManager<IdentityRole> roleManager,
										UserManager<ApplicationUser> userManager,
										ILogger<AdministrationController> logger)
		{
			this.roleManager = roleManager;
			this.userManager = userManager;
			this.logger = logger;
		}
		[HttpGet]
		[Authorize(Policy = "CreateRolePolicy")]
		public IActionResult CreateRole()
		{
			return View();
		}
		[HttpPost]
		[Authorize(Policy = "CreateRolePolicy")]
		public async Task<IActionResult> CreateRole(CreateRoleViewModel model)
		{
			if (ModelState.IsValid)
			{
				IdentityRole identityRole = new IdentityRole
				{
					Name = model.RoleName
				};

				IdentityResult result = await roleManager.CreateAsync(identityRole);
				if (result.Succeeded)
				{
					return RedirectToAction("ListRoles", "Administration");
				}
				foreach (IdentityError error in result.Errors)
				{
					ModelState.AddModelError("", error.Description);
				}

			}
			return View(model);
		}
		[HttpGet]
		public IActionResult ListRoles()
		{
			var roles = roleManager.Roles;
			return View(roles);
		}
		[HttpGet]
		[Authorize(Policy = "EditRolePolicy")]
		public async Task<IActionResult> EditRole(string Id)
		{
			var role = await roleManager.FindByIdAsync(Id);

			if (role == null)
			{
				ViewBag.ErrorMessage = $"Role with id = {Id} cannot be found!";

				return View("NotFound");
			}

			var model = new EditRoleViewModel
			{
				Id = role.Id,
				RoleName = role.Name
			};

			foreach (var user in userManager.Users)
			{
				if (await userManager.IsInRoleAsync(user, role.Name))
				{
					model.Users.Add(user.UserName);
				}
			}

			return View(model);
		}
		[HttpPost]
		[Authorize(Policy = "EditRolePolicy")]
		public async Task<IActionResult> EditRole(EditRoleViewModel model)
		{
			var role = await roleManager.FindByIdAsync(model.Id);

			if (role == null)
			{
				ViewBag.ErrorMessage = $"Role with id = {model.Id} cannot be found!";

				return View("NotFound");
			}
			else
			{
				role.Name = model.RoleName;
				var result = await roleManager.UpdateAsync(role);

				if (result.Succeeded)
				{
					return RedirectToAction("ListRoles");
				}
				foreach (var error in result.Errors)
				{
					ModelState.AddModelError("", error.Description);
				}
				return View(model);
			}

		}
		[HttpGet]
		[Authorize(Policy = "EditRolePolicy")]

		public async Task<IActionResult> EditRolesForUser(string userId)
		{
			ViewBag.userId = userId;

			var user = await userManager.FindByIdAsync(userId);
			if (user == null)
			{
				ViewBag.Message = $"User with this Id={userId} not found!";
				return View("NotFound");
			}
			var model = new List<EditRolesForUserViewModel>();

			foreach (var role in roleManager.Roles)
			{
				var editRolesForUserViewModel = new EditRolesForUserViewModel
				{
					RoleId = role.Id,
					RoleName = role.Name
				};

				if (await userManager.IsInRoleAsync(user, role.Name))
				{
					editRolesForUserViewModel.IsSelected = true;
				}
				else
				{
					editRolesForUserViewModel.IsSelected = false;
				}

				model.Add(editRolesForUserViewModel);
			}
			return View(model);


		}
		[HttpPost]
		[Authorize(Policy = "EditRolePolicy")]

		public async Task<IActionResult> EditRolesForUser(List<EditRolesForUserViewModel> model, string userId)
		{

			var user = await userManager.FindByIdAsync(userId);
			if (user == null)
			{
				ViewBag.Message = $"User with this Id = {userId} not found!";
				return View("NotFound");
			}
			var roles = await userManager.GetRolesAsync(user);
			var result = await userManager.RemoveFromRolesAsync(user, roles);

			if (!result.Succeeded)
			{
				ModelState.AddModelError("", "Cannot remove from existing roles");
				return View(model);

			}
			result = await userManager.AddToRolesAsync(user,
									  model.Where(x => x.IsSelected).Select(y => y.RoleName));
			if (!result.Succeeded)
			{
				ModelState.AddModelError("", "Cannot add selected roles to user");
				return View(model);

			}
			return RedirectToAction("EditUser", new { Id = userId });

		}
		[HttpGet]
		public async Task<IActionResult> EditUsersInRole(string roleId)
		{
			ViewBag.roleId = roleId;

			var role = await roleManager.FindByIdAsync(roleId);

			if (role == null)
			{
				ViewBag.Message = $"Role with this Id = {roleId} not found!";
				return View("NotFound");
			}
			var model = new List<EditUsersInRoleViewModel>();

			foreach (var user in userManager.Users)
			{
				var editUserInrolesViewMdole = new EditUsersInRoleViewModel
				{
					UserId = user.Id,
					UserName = user.UserName
				};
				if (await userManager.IsInRoleAsync(user, role.Name))
				{
					editUserInrolesViewMdole.IsSelected = true;
				}
				else
				{
					editUserInrolesViewMdole.IsSelected = false;

				}
				model.Add(editUserInrolesViewMdole);
			}
			return View(model);
		}
		[HttpPost]
		public async Task<IActionResult> EditUsersInRole(List<EditUsersInRoleViewModel> model, string roleId)
		{

			var role = await roleManager.FindByIdAsync(roleId);

			if (role == null)
			{
				ViewBag.Message = $"Role with this Id = {roleId} not found!";
				return View("NotFound");
			}
			for (int i = 0; i < model.Count; i++)
			{
				var user = await userManager.FindByIdAsync(model[i].UserId);

				IdentityResult result = null;

				if (model[i].IsSelected && !(await userManager.IsInRoleAsync(user, role.Name)))
				{
					result = await userManager.AddToRoleAsync(user, role.Name);
				}
				else if (!model[i].IsSelected && await userManager.IsInRoleAsync(user, role.Name))
				{
					result = await userManager.RemoveFromRoleAsync(user, role.Name);

				}
				else
				{
					continue;
				}
				if (result.Succeeded)
				{
					if (i < (model.Count - 1))
						continue;
					else
						return RedirectToAction("EditRole", new { Id = roleId });
				}
			}
			return RedirectToAction("EditRole", new { Id = roleId });

		}
		[HttpGet]
		public IActionResult ListUsers()
		{
			var users = userManager.Users;

			return View(users);
		}
		[HttpGet]
		public async Task<IActionResult> EditUser(string id)
		{
			var user = await userManager.FindByIdAsync(id);

			if (user == null)
			{
				ViewBag.Message = $"User with this Id = {id} not found!";
				return View("NotFound");
			}
			var userRoles = await userManager.GetRolesAsync(user);
			var userClaims = await userManager.GetClaimsAsync(user);

			var model = new EditUserViewModel
			{
				Id = user.Id,
				UserName = user.UserName,
				Email = user.Email,
				City = user.City,
				Roles = userRoles,
				Claims = userClaims.Select(c=>c.Value).ToList()
				// to show claim type and claim value saperate
				//Claims = userClaims.Select(c => c.Type + " : " + c.Value).ToList()
			};
			return View(model);

		}
		[HttpPost]
		public async Task<IActionResult> EditUser(EditUserViewModel model)
		{
			var user = await userManager.FindByIdAsync(model.Id);

			if (user == null)
			{
				ViewBag.Message = $"User with this Id = {model.Id} not found!";
				return View("NotFound");
			}
			else
			{
				user.UserName = model.UserName;
				user.Email = model.Email;
				user.City = model.City;

				var result = await userManager.UpdateAsync(user);

				if (result.Succeeded)
				{
					return RedirectToAction("ListUsers", "Administration");
				}
				foreach (var error in result.Errors)
				{
					ModelState.AddModelError("", error.Description);
				}
				return View(model);

			}
		}
		[HttpPost]
		public async Task<IActionResult> DeleteUser(string id)
		{
			var user = await userManager.FindByIdAsync(id);

			if (user == null)
			{
				ViewBag.Message = $"User with this Id = {id} not found!";
				return View("NotFound");
			}
			else
			{
				var result = await userManager.DeleteAsync(user);

				if (result.Succeeded)
				{
					return RedirectToAction("ListUsers");
				}
				foreach (var error in result.Errors)
				{
					ModelState.AddModelError("", error.Description);
				}
				return View("ListUsers");
			}
		}
		[HttpPost]
		[Authorize(Policy = "DeleteRolePolicy")]
		public async Task<IActionResult> DeleteRole(string id)
		{
			var role = await roleManager.FindByIdAsync(id);

			if (role == null)
			{
				ViewBag.Message = $"Role with this Id = {id} not found!";
				return View("NotFound");
			}
			else
			{
				try
				{
					//throw new Exception("Test Exception");

					var result = await roleManager.DeleteAsync(role);

					if (result.Succeeded)
					{
						return RedirectToAction("ListRoles");
					}
					foreach (var error in result.Errors)
					{
						ModelState.AddModelError("", error.Description);
					}
					return View("ListRoles");
				}
				catch (DbUpdateException ex)
				{
					logger.LogError($"Error deleting role {ex}");

					ViewBag.ErrorTitle = $"{role.Name} role is in use";
					ViewBag.ErrorMessage = $"{role.Name} role cannto be deleted as there are users in this role," +
						$" if you want to delete role" +
						$" first remove users from the role and then try to delete.";
					return View("Error");
				}
			}
		}
		[HttpGet]
		public async Task<IActionResult> ManageUserClaims(string userId)
		{
			var user = await userManager.FindByIdAsync(userId);

			if (user == null)
			{
				ViewBag.ErrorMessage = $"User with Id = {userId} cannot be found!";
				return View("NotFound");
			}

			var existingUserClaims = await userManager.GetClaimsAsync(user);

			var model = new UserClaimsViewModel
			{
				UserId = userId
			};
			foreach (Claim claim in ClaimsStore.AllClaims)
			{
				UserClaim userClaim = new UserClaim
				{
					ClaimsType = claim.Type
				};
				// if the user have existing claims , set IsSelected property to true otherwise set it to false

				if (existingUserClaims.Any(c => c.Type == claim.Type))

				//if (existingUserClaims.Any(c => c.Type == claim.Type && c.Value == "true"))
				{
					userClaim.IsSelected = true;
				}

				model.Claims.Add(userClaim);
			}
			return View(model);
		}
		[HttpPost]
		public async Task<IActionResult> ManageUserClaims(UserClaimsViewModel model)
		{
			var user = await userManager.FindByIdAsync(model.UserId);

			if (user == null)
			{
				ViewBag.ErrorMessage = $"User with Id = {model.UserId} cannot be found!";
				return View("NotFound");
			}

			var claims = await userManager.GetClaimsAsync(user);
			var result = await userManager.RemoveClaimsAsync(user, claims);

			if (!result.Succeeded)
			{
				ModelState.AddModelError("", "Cannot remove existing userclaims");
				return View();
			}
			//claim type and claim value method
			//result = await userManager.AddClaimsAsync(user,
				          //model.Claims.Select(c => new Claim(c.ClaimsType, c.IsSelected ? "true" : "false")));

			result = await userManager.AddClaimsAsync(user,
				                model.Claims.Where(c=>c.IsSelected).Select(c => new Claim(c.ClaimsType, c.ClaimsType)));

			if (!result.Succeeded)
			{
				ModelState.AddModelError("", "Cannot add selected userclaims");
				return View();
			}
			return RedirectToAction("EditUser", new { Id = model.UserId });
		}
	}
}