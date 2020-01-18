using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace KellyRecruitment.Controllers
{
	public class ErrorController : Controller
	{
		private readonly ILogger<ErrorController> logger;

		public ErrorController(ILogger<ErrorController> logger)
		{
			this.logger = logger;
		}
		[Route("Error/{statusCode}")]
		public IActionResult HttpStatusCodeHandler(int statusCode)

		{
			var statusCodeResult = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();
			switch (statusCode)
			{
				case 404:
					ViewBag.ErrorMessage = "Sorry, the resourcec you requested could not found";
					logger.LogWarning($"404 Error ouccred. Path{statusCodeResult.OriginalPath}" +
						$"and QueryString = {statusCodeResult.OriginalQueryString}");

					//ViewBag.Path = statusCodeResult.OriginalPath;
					//ViewBag.QS = statusCodeResult.OriginalQueryString;
					break;

			}
			return View("NotFound");
		}
		[Route("Error")]
		[AllowAnonymous]
		public IActionResult Error()
		{
			var exceptionDetails = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

			logger.LogError($"The Path {exceptionDetails.Path} threw an exection{exceptionDetails.Error}");
			//ViewBag.ExceptionPath = exceptionDetails.Path;
			//ViewBag.ExceptionMessage = exceptionDetails.Error.Message;
			//ViewBag.Stacktrace = exceptionDetails.Error.StackTrace;


			return View("Error");
		}

	}
}