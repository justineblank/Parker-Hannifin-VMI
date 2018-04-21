namespace RFIDApplication.DAL.Interfaces
{
    public interface IUnauthenticatedHttpContextAccessor
    {
        bool HasCompany { get; }
        bool HasTimeZoneOffset { get; }
        string Company { get; }
        int? TimeZoneOffset { get; }
    }
}
