using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using RFIDApplication.DAL.Models;

namespace RFIDApplication.DAL.Repositories
{
    public interface IReadersRepository
    {
        Task<List<ReadersModel>> ReadAsync();
        Task<ReadersModel> ReaderDetailAsync(string readerId);        
        Task<ReadersModel> ReadersEditAsync(ReadersEditModel readersEditModel);
    }
}
