using Microsoft.AspNetCore.Http;
using RFIDApplication.DAL.Interfaces;

namespace RFIDApplication
{
    public class UnauthenticatedHttpContextAccessor : IUnauthenticatedHttpContextAccessor
	{
		private IHttpContextAccessor _httpContextAccessor;

		public UnauthenticatedHttpContextAccessor(IHttpContextAccessor httpContextAccessor)
		{
			_httpContextAccessor = httpContextAccessor;
		}

		public bool HasCompany
		{
			get
			{
				bool hasCompany = false;

				try
				{
					if (_httpContextAccessor.HttpContext.Request.Form.ContainsKey("company"))
					{
						if (!string.IsNullOrWhiteSpace(_httpContextAccessor.HttpContext.Request.Form["company"]))
						{
							hasCompany = true;
						}
					}
				}
				catch
				{
                    if (_httpContextAccessor.HttpContext.Request.Cookies.ContainsKey("company"))
                    {
                        hasCompany = true;
                    }
                    else
                        hasCompany = false;
				}
				return hasCompany;
			}
		}

		public bool HasTimeZoneOffset
		{
			get
			{
				bool hasTimeZoneOffset = false;

				try
				{
					if (_httpContextAccessor.HttpContext.Request.Form.ContainsKey("timezoneoffset"))
					{
						if (!string.IsNullOrWhiteSpace(_httpContextAccessor.HttpContext.Request.Form["timezoneoffset"]))
						{
							hasTimeZoneOffset = true;
						}
					}
				}
				catch
				{
					hasTimeZoneOffset = false;
				}
				return hasTimeZoneOffset;
			}
		}

		public string Company
		{
			get
			{
				string company = string.Empty;

				if (HasCompany)
				{
                    try
                    {
                        company = _httpContextAccessor.HttpContext.Request.Form["company"];
                    }
                    catch
                    {
                        company = _httpContextAccessor.HttpContext.Request.Cookies["company"];
                    }

                }
				return company;
			}
		}

		public int? TimeZoneOffset
		{
			get
			{
				int tzOffset;

				if (HasTimeZoneOffset)
				{
					if (int.TryParse(_httpContextAccessor.HttpContext.Request.Form["timezoneoffset"], out tzOffset))
					{
						return tzOffset;
					}
				}

				return null;
			}
		}
	}
}
