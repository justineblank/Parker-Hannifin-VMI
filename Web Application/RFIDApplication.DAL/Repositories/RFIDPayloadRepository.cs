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
    public class RFIDPayloadRepository : BaseTypeRepository<RFIDPayloadModel>, IRFIDPayloadRepository
    {
        public RFIDPayloadRepository
            (
            IConfigurationRoot config,
            ICurrentPrincipalAccessor currentPrincipalAccessor,
            IUnauthenticatedHttpContextAccessor unauthenticatedHttpContextAccessor
            ) : base(config, currentPrincipalAccessor, unauthenticatedHttpContextAccessor)
        {

        }
        //
        public async Task<RFIDPayloadResponseModel> CreateAsync(RFIDPayloadModel model, SqlConnection connection = null, SqlTransaction transaction = null)
        {
            bool commitTransaction = (transaction == null);
            DateTime currentDateUtc = DateTime.UtcNow;
            int intReaderID = 0;
            List<Tags> lst_Tags = null;
            //
            RFIDPayloadResponseModel obj_RFIDPayloadResponseModel = null;
            TagsResponse obj_TagsResponse = null;
            List<TagsResponse> lst_tagsResponse = null;
            //
            string str_message = string.Empty;
            if (model == null)
            {
                throw new ArgumentNullException("RFID Reader Repository");
            }

            try
            {
                if (connection == null)
                {
                    connection = new SqlConnection(this.ConnectionString);
                }

                if (connection.State != ConnectionState.Open)
                {
                    await connection.OpenAsync();
                }

                //Tran1 - start
                if (transaction == null)
                {
                    transaction = connection.BeginTransaction();
                }

                using (SqlCommand cmd = new SqlCommand("[Proc_Reader_Add]", connection, transaction))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", intReaderID);
                    cmd.Parameters["@Id"].Direction = ParameterDirection.Output;
                    cmd.Parameters.AddWithValue("@readerId", model.readerId);
                    cmd.Parameters.AddWithValue("@location", String.Empty);
                    cmd.Parameters.AddWithValue("@lastSeen", DateTime.UtcNow);
                    //
                    await cmd.ExecuteNonQueryAsync();

                    //await cmd.ExecuteNonQueryAsync();

                    if (!int.TryParse(cmd.Parameters["@Id"].Value.ToString(), out intReaderID))
                    {
                        throw new Exception("Unable to add reader.");
                    }
                }
                if (commitTransaction)
                {
                    transaction.Commit();
                    transaction = null;
                }
                //Tran1 - end
                //Tran2 - start
                lst_Tags = model.tags;
                obj_RFIDPayloadResponseModel = new RFIDPayloadResponseModel();
                if (lst_Tags != null && lst_Tags.Count > 0)
                {
                    lst_tagsResponse = new List<TagsResponse>();
                    //
                    if (transaction == null)
                    {
                        transaction = connection.BeginTransaction();
                    }
                    // save
                    foreach (Tags obj_Tag in lst_Tags)
                    {
                        str_message = string.Empty;
                        //
                        obj_TagsResponse = new TagsResponse();
                        obj_TagsResponse.readId = obj_Tag.readId;

                        using (SqlCommand cmd = new SqlCommand("[Proc_payload_Add]", connection, transaction))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@readerId", model.readerId);
                            //cmd.Parameters["@p_ReportId"].Direction = ParameterDirection.Output;
                            cmd.Parameters.AddWithValue("@location", string.Empty);
                            cmd.Parameters.AddWithValue("@lastSeen", DateTime.UtcNow);
                            //-->
                            cmd.Parameters.AddWithValue("@antenna", obj_Tag.antenna);
                            cmd.Parameters.AddWithValue("@epc", obj_Tag.epc);
                            cmd.Parameters.AddWithValue("@timestamp"
                                            , UnixTimeStampToDateTime(obj_Tag.timestamp)
                                );
                            cmd.Parameters.AddWithValue("@syncStatus", string.Empty);
                            cmd.Parameters.AddWithValue("@message", string.Empty);
                            cmd.Parameters["@message"].Direction = ParameterDirection.Output;
                            //
                            await cmd.ExecuteNonQueryAsync();
                            //
                            str_message = cmd.Parameters["@message"].Value.ToString();
                            
                        }
                        if (str_message == string.Empty)
                        {
                            obj_TagsResponse.status = "success";
                            obj_TagsResponse.message = string.Empty;
                        }
                        else
                        {
                            obj_TagsResponse.status = "fail";
                            obj_TagsResponse.message = "epc already present";
                        }
                        lst_tagsResponse.Add(obj_TagsResponse);
                    }
                }
                //
                obj_RFIDPayloadResponseModel.status = "Success";
                obj_RFIDPayloadResponseModel.message = "";
                if (lst_Tags != null)
                {
                    obj_RFIDPayloadResponseModel.tagCount = lst_Tags.Count;
                    obj_RFIDPayloadResponseModel.tags = lst_tagsResponse;
                }
                //
                if (commitTransaction)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }
                //Tran2 - end
            }
            catch (Exception ex)
            {
                if (transaction.Connection != null) transaction.Rollback();
                throw ex;
            }
            finally
            {
                if (commitTransaction)
                {
                    if (transaction != null)
                        ((IDisposable)transaction).Dispose();

                    if (connection != null)
                        ((IDisposable)connection).Dispose();
                }
            }

            return obj_RFIDPayloadResponseModel;
        }

        #region "Private"
        private DateTime UnixTimeStampToDateTime(string unixTimeStamp)
        {
            if (unixTimeStamp.Trim() == string.Empty)
            {
                unixTimeStamp = "0";
            }
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(Convert.ToDouble(unixTimeStamp));//.ToLocalTime();
            return dtDateTime;
        }
        #endregion

    }
}
