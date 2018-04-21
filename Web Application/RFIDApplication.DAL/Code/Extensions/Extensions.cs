using Microsoft.Extensions.Configuration;
using System;
using System.Data.SqlClient;
using System.Security.Claims;

namespace RFIDApplication.DAL.Extensions
{
    static class DateTimeExtensions
	{
		public static DateTime ToUtc(this DateTime local, int tzOffset)
		{
			return local.AddMinutes(tzOffset);
		}

        public static DateTime FirstDayOfWeek(this DateTime dt, DayOfWeek startOfWeek)
        {
            int diff = dt.DayOfWeek - startOfWeek;
            if (diff < 0)
            {
                diff += 7;
            }
            return dt.AddDays(-1 * diff).Date;
        }

        public static DateTime FirstDayOfMonth(this DateTime value)
        {
            return new DateTime(value.Year, value.Month, 1);
        }

        public static int DaysInMonth(this DateTime value)
        {
            return DateTime.DaysInMonth(value.Year, value.Month);
        }

        public static DateTime LastDayOfMonth(this DateTime value)
        {
            return new DateTime(value.Year, value.Month, value.DaysInMonth());
        }
    }


    static class IdentityExtensions
	{
		public static string GetCompany(this ClaimsPrincipal identity)
		{
			if (object.ReferenceEquals(identity.GetType(), typeof(ClaimsPrincipal)))
			{
				Claim claim = ((ClaimsIdentity)identity.Identity).FindFirst("company");

				if ((claim != null))
				{
					return claim.Value;
				}
			}
			return string.Empty;
		}

		public static string GetConnectionString(this ClaimsPrincipal identity, IConfigurationRoot config)
		{
			if (!string.IsNullOrWhiteSpace(identity.GetCompany()))
			{
				SqlConnectionStringBuilder sqlCnxStringBuilder = new SqlConnectionStringBuilder(config.GetConnectionString("RFIDConnection"));

				sqlCnxStringBuilder.InitialCatalog = identity.GetCompany();

				return sqlCnxStringBuilder.ConnectionString;
			}
			return string.Empty;
		}

        public static string GetLocaleID(this ClaimsPrincipal identity)
        {
            if (object.ReferenceEquals(identity.GetType(), typeof(ClaimsPrincipal)))
            {
                Claim claim = ((ClaimsIdentity)identity.Identity).FindFirst("localeid");

                if ((claim != null))
                {
                    return claim.Value;
                }
            }
            return string.Empty;
        }

        //public static string GetLocationID(this ClaimsPrincipal identity)
        //{
        //    if (object.ReferenceEquals(identity.GetType(), typeof(ClaimsPrincipal)))
        //    {
        //        Claim claim = ((ClaimsIdentity)identity.Identity).FindFirst("locationid");

        //        if ((claim != null))
        //        {
        //            return claim.Value;
        //        }
        //    }
        //    return string.Empty;
        //}
        public static int? GetLocationID(this ClaimsPrincipal identity)
        {
            if (object.ReferenceEquals(identity.GetType(), typeof(ClaimsPrincipal)))
            {
                Claim claim = ((ClaimsIdentity)identity.Identity).FindFirst("locationid");

                if ((claim != null))
                {
                    int locationID;
                    if (int.TryParse(claim.Value, out locationID))
                    {
                        return locationID;
                    }
                }
            }
            return null;
        }

        public static int? GetTimeZoneOffset(this ClaimsPrincipal identity)
		{
			if (object.ReferenceEquals(identity.GetType(), typeof(ClaimsPrincipal)))
			{
				Claim claim = ((ClaimsIdentity)identity.Identity).FindFirst("timezoneoffset");

				if ((claim != null))
				{
					int tzOffset;
					if (int.TryParse(claim.Value, out tzOffset))
					{
						return tzOffset;
					}
				}
			}
			return null;
		}

		public static string GetUserID(this ClaimsPrincipal identity)
		{
			if (object.ReferenceEquals(identity.GetType(), typeof(ClaimsPrincipal)))
			{
				Claim claim = ((ClaimsIdentity)identity.Identity).FindFirst("userid");

				if ((claim != null))
				{
					return claim.Value;
				}
			}
			return string.Empty;
		}
	}
}
