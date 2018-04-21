using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using RFIDApplication.DAL.Models;

namespace RFIDApplication.DAL.Interfaces
{
    public interface IScansRepository
    {
        Task<List<ScansModel>> ReadAsync();
    }
}
