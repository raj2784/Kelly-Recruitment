using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace KellyRecruitment.Security
{
	public class CustomEmailConfirmationTokenProvider<TUser> :
								DataProtectorTokenProvider<TUser> where TUser : class
	{
		public CustomEmailConfirmationTokenProvider(IDataProtectionProvider dataProtectionProvider,
			IOptions<CustomEmailConfirmationTokenProviderOptions> options) : base(dataProtectionProvider, options)
		{

		}

	}
}
