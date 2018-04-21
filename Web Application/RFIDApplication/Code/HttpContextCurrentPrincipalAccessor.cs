using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using RFIDApplication.DAL.Interfaces;

namespace RFIDApplication
{
    public class HttpContextCurrentPrincipalAccessor : ICurrentPrincipalAccessor
	{
		private IHttpContextAccessor _httpContextAccessor;

		public HttpContextCurrentPrincipalAccessor(IHttpContextAccessor httpContextAccessor)
		{
			_httpContextAccessor = httpContextAccessor;
		}

		public ClaimsPrincipal CurrentPrincipal
		{
			get
			{
				return _httpContextAccessor.HttpContext.User;
			}
		}
	}
}
