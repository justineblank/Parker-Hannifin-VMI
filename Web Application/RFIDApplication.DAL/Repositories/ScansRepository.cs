using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Dynamic;
using System.Threading.Tasks;
using RFIDApplication.DAL.Interfaces;
using RFIDApplication.DAL.Models;

namespace RFIDApplication.DAL.Repositories
{
    public class ScansRepository : BaseTypeRepository<ScansModel>, IScansRepository
    {
        public ScansRepository
           (
           IConfigurationRoot config,
           ICurrentPrincipalAccessor currentPrincipalAccessor,
           IUnauthenticatedHttpContextAccessor unauthenticatedHttpContextAccessor
           ) : base(config, currentPrincipalAccessor, unauthenticatedHttpContextAccessor)
        {

        }

        public async Task<List<ScansModel>> ReadAsync()
        {
            using (SqlConnection connection = new SqlConnection(this.ConnectionString))
            {
                await connection.OpenAsync();

                using (SqlCommand cmd = new SqlCommand("[Proc_Scans_Get]", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    SqlDataReader dr = await cmd.ExecuteReaderAsync();

                    //return await MapDataToModelAsync(dr);
                    return await MapDataToModelListAsync<ScansModel>(dr);
                }
            }
        }
    }
}
