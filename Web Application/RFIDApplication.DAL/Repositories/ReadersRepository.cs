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
    public class ReadersRepository : BaseTypeRepository<ReadersModel>, IReadersRepository
    {
        public ReadersRepository
            (
            IConfigurationRoot config,
            ICurrentPrincipalAccessor currentPrincipalAccessor,
            IUnauthenticatedHttpContextAccessor unauthenticatedHttpContextAccessor
            ) : base(config, currentPrincipalAccessor, unauthenticatedHttpContextAccessor)
        {

        }

        public async Task<List<ReadersModel>> ReadAsync()
        {
            using (SqlConnection connection = new SqlConnection(this.ConnectionString))
            {
                await connection.OpenAsync();

                using (SqlCommand cmd = new SqlCommand("[Proc_Readers_Get]", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    SqlDataReader dr = await cmd.ExecuteReaderAsync();

                    //return await MapDataToModelAsync(dr);
                    return await MapDataToModelListAsync<ReadersModel>(dr);
                }
            }
        }

        //
        public async Task<ReadersModel> ReaderDetailAsync(string readerId)
        {
            using (SqlConnection connection = new SqlConnection(this.ConnectionString))
            {
                await connection.OpenAsync();

                using (SqlCommand cmd = new SqlCommand("[Proc_Readers_Get]", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@readerId", readerId);

                    SqlDataReader dr = await cmd.ExecuteReaderAsync();

                    return await MapDataToModelAsync(dr);
                    //return await MapDataToModelListAsync<ReadersModel>(dr);
                }
            }
        }

        public async Task<ReadersModel> ReadersEditAsync(ReadersEditModel readersEditModel)
        {
            SqlTransaction transaction = null;
            SqlConnection connection = null;
            try
            {
                using (connection = new SqlConnection(this.ConnectionString))
                {

                    await connection.OpenAsync();

                    if (transaction == null)
                    {
                        transaction = connection.BeginTransaction();
                    }

                    using (SqlCommand cmd = new SqlCommand("[Proc_Readers_Edit]", connection, transaction))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@id", readersEditModel.id);
                        cmd.Parameters.AddWithValue("@location", readersEditModel.location);

                        await cmd.ExecuteNonQueryAsync();
                        //return await MapDataToModelListAsync<ReadersModel>(dr);
                    }


                    transaction.Commit();

                    return await ReaderDetailAsync(readersEditModel.readerId);

                }
            }
            catch (Exception ex)
            {
                if (transaction.Connection != null) transaction.Rollback();
                throw ex;
            }
            finally
            {
                
                    if (transaction != null)
                        ((IDisposable)transaction).Dispose();

                    if (connection != null)
                        ((IDisposable)connection).Dispose();
                
            }
        }
    }
}
