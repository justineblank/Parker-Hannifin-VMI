using System.Security.Claims;

namespace RFIDApplication.DAL.Interfaces
{
	public interface ICurrentPrincipalAccessor
	{
		ClaimsPrincipal CurrentPrincipal { get; }
	}
}
