using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using RFIDApplication.DAL.Models;

namespace RFIDApplication.DAL.Repositories
{
    public interface IRFIDPayloadRepository
    {
        Task<RFIDPayloadResponseModel> CreateAsync(RFIDPayloadModel model, SqlConnection connection = null, SqlTransaction transaction = null);
        
    }
}
