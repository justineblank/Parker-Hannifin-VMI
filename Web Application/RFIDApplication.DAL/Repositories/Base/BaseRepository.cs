using Microsoft.Extensions.Configuration;
using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using RFIDApplication.DAL.Extensions;
using RFIDApplication.DAL.Interfaces;

namespace RFIDApplication.DAL.Repositories
{
    public abstract class BaseRepository
	{
		//Implements IDisposable
		protected readonly IConfigurationRoot _config;
		protected readonly ICurrentPrincipalAccessor _currentPrincipalAccessor;
		private readonly IUnauthenticatedHttpContextAccessor _unauthenticatedHttpContextAccessor;

		public BaseRepository(IConfigurationRoot config, ICurrentPrincipalAccessor currentPrincipalAccessor, IUnauthenticatedHttpContextAccessor unauthenticatedHttpContextAccessor)
		{
			_config = config;
			_currentPrincipalAccessor = currentPrincipalAccessor;
			_unauthenticatedHttpContextAccessor = unauthenticatedHttpContextAccessor;
		}

		protected string BaseConnectionString
		{
			get
			{
				return _config.GetConnectionString("RFIDConnection");
			}
		}

		protected string Company
		{
			get
			{
				if (!string.IsNullOrWhiteSpace(_currentPrincipalAccessor.CurrentPrincipal.GetCompany()))
				{
					return _currentPrincipalAccessor.CurrentPrincipal.GetCompany();
				}
				else if (!string.IsNullOrWhiteSpace(_unauthenticatedHttpContextAccessor.Company))
				{
					return _unauthenticatedHttpContextAccessor.Company;
				}

				return string.Empty;
			}
		}

		protected string ConnectionString
		{
			get
			{
                //if (!string.IsNullOrWhiteSpace(_currentPrincipalAccessor.CurrentPrincipal.GetCompany()))
                //{
                //    return _currentPrincipalAccessor.CurrentPrincipal.GetConnectionString(_config);
                //}
                //else
                //            if (!string.IsNullOrWhiteSpace(_unauthenticatedHttpContextAccessor.Company))
                //{
                //    SqlConnectionStringBuilder sqlcnxstringbuilder = new SqlConnectionStringBuilder(this.BaseConnectionString);

                //    sqlcnxstringbuilder.InitialCatalog = _unauthenticatedHttpContextAccessor.Company;

                //    return sqlcnxstringbuilder.ConnectionString;
                //}

                //return string.Empty;

                return this.BaseConnectionString;

            }
		}

        protected string LocaleID
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(_currentPrincipalAccessor.CurrentPrincipal.GetLocaleID()))
                {
                    return _currentPrincipalAccessor.CurrentPrincipal.GetLocaleID();
                }

                return string.Empty;
            }
        }

        protected int TimeZoneOffset
		{
			get
			{
				if (_currentPrincipalAccessor.CurrentPrincipal.GetTimeZoneOffset().HasValue)
				{
					return _currentPrincipalAccessor.CurrentPrincipal.GetTimeZoneOffset().Value;
				}
				else if (_unauthenticatedHttpContextAccessor.TimeZoneOffset.HasValue)
				{
					return _unauthenticatedHttpContextAccessor.TimeZoneOffset.Value;
				}

				throw new Exception("Unable to resolve time zone.");
			}
		}

		protected DateTime LocalToUtc(DateTime local)
		{
			return local.AddMinutes(TimeZoneOffset);
		}

        protected static IEnumerable<SqlDataRecord> CreateSqlDataRecords(IEnumerable<string> stringIds)
        {
            SqlMetaData[] metaData = new SqlMetaData[1];
            metaData[0] = new SqlMetaData("varId", SqlDbType.NVarChar, 100);
            SqlDataRecord record = new SqlDataRecord(metaData);

            foreach (string stringId in stringIds.Distinct())
            {
                record.SetSqlString(0, stringId.Trim());
                yield return record;
            }
        }

        protected static IEnumerable<SqlDataRecord> CreateSqlDataRecords(IEnumerable<int> intIds)
        {
            SqlMetaData[] metaData = new SqlMetaData[1];
            metaData[0] = new SqlMetaData("intId", SqlDbType.Int);
            SqlDataRecord record = new SqlDataRecord(metaData);

            foreach (int intId in intIds.Distinct())
            {
                record.SetInt32(0, intId);
                yield return record;
            }
        }

        //protected static IEnumerable<SqlDataRecord> CreateSqlDataRecords(IDictionary<string, string> parameters)
        //{
        //    SqlMetaData[] metaData = new SqlMetaData[1];
        //    metaData[0] = new SqlMetaData("varId", SqlDbType.NVarChar, 100);
        //    SqlDataRecord record = new SqlDataRecord(metaData);

        //    foreach (string stringId in stringIds.Distinct())
        //    {
        //        record.SetSqlString(0, stringId.Trim());
        //        yield return record;
        //    }
        //}

        //      protected T MapDataToModel(DbDataReader dr)
        //{
        //	return MapDataToModelListAsync(dr).FirstOrDefault();
        //}

        //protected List<T> MapDataToModelListAsync(DbDataReader dr)
        //{
        //		return MapDataToModelList<T>(dr);
        //}

        //protected List<ModelType> MapDataToModelList<ModelType>(DbDataReader dr) where ModelType : new()
        //{
        //	Type entityType = typeof(ModelType);
        //	List<ModelType> entityList = new List<ModelType>();
        //	Hashtable hashtable = new Hashtable();
        //	PropertyInfo[] properties = entityType.GetProperties();

        //          try
        //          {
        //	    foreach (PropertyInfo info in properties)
        //	    {
        //		    hashtable[info.Name.ToUpper()] = info;
        //	    }

        //	    while (dr.Read())
        //	    {
        //		    ModelType newObject = new ModelType();
        //		    for (int index = 0; index <= dr.FieldCount - 1; index++)
        //		    {
        //			    PropertyInfo info = (PropertyInfo)hashtable[dr.GetName(index).Substring(3).ToUpper()];
        //			    if ((info != null) && info.CanWrite && (dr.GetValue(index) != DBNull.Value))
        //			    {
        //				    Type fieldType = dr.GetFieldType(index);

        //				    if (fieldType.Name ==  "DateTime")
        //				    {
        //					    DateTime dt = (DateTime)dr.GetValue(index);
        //					    info.SetValue(newObject, dt.AddMinutes(-1 * this.TimeZoneOffset), null);
        //				    }
        //				    else
        //				    {
        //					    info.SetValue(newObject, dr.GetValue(index), null);
        //				    }
        //			    }
        //		    }
        //		    entityList.Add(newObject);
        //	    }
        //          }
        //          catch (Exception ex)
        //          {
        //              throw ex;
        //          }
        //          finally
        //          {
        //              dr.Dispose();
        //          }

        //          return entityList;
        //}

        //      protected async Task<T> MapDataToModelAsync(DbDataReader dr)
        //      {
        //          Type entityType = typeof(T);
        //          T entity = new T();
        //          Hashtable hashtable = new Hashtable();
        //          PropertyInfo[] properties = entityType.GetProperties();

        //          try
        //          {
        //              foreach (PropertyInfo info in properties)
        //              {
        //                  hashtable[info.Name.ToUpper()] = info;
        //              }

        //              if (await dr.ReadAsync())
        //              {
        //                  T newObject = new T();
        //                  for (int index = 0; index <= dr.FieldCount - 1; index++)
        //                  {
        //                      PropertyInfo info = (PropertyInfo)hashtable[dr.GetName(index).Substring(3).ToUpper()];
        //                      if ((info != null) && info.CanWrite && (dr.GetValue(index) != DBNull.Value))
        //                      {
        //                          info.SetValue(newObject, dr.GetValue(index), null);
        //                      }
        //                  }
        //                  entity = newObject;
        //              }
        //          }
        //          catch (Exception ex)
        //          {
        //              throw ex;
        //          }
        //          finally
        //          {
        //              dr.Dispose();
        //          }

        //          return entity;
        //      }

        //      protected async Task<List<ModelType>> MapDataToModelListAsync<ModelType>(DbDataReader dr) where ModelType : new()
        //{
        //	Type entityType = typeof(ModelType);
        //	List<ModelType> entityList = new List<ModelType>();
        //	Hashtable hashtable = new Hashtable();
        //	PropertyInfo[] properties = entityType.GetProperties();

        //          try
        //          {

        //              foreach (PropertyInfo info in properties)
        //              {
        //                  hashtable[info.Name.ToUpper()] = info;
        //              }

        //              while (await dr.ReadAsync())
        //              {
        //                  ModelType newObject = new ModelType();
        //                  for (int index = 0; index <= dr.FieldCount - 1; index++)
        //                  {
        //                      PropertyInfo info = (PropertyInfo)hashtable[dr.GetName(index).Substring(3).ToUpper()];
        //                      if ((info != null) && info.CanWrite && (dr.GetValue(index) != DBNull.Value))
        //                      {
        //                          info.SetValue(newObject, dr.GetValue(index), null);
        //                      }
        //                  }
        //                  entityList.Add(newObject);
        //              }
        //          }
        //          catch (Exception ex)
        //          {
        //             throw ex;
        //          }
        //          finally
        //          {
        //              dr.Dispose();
        //          }

        //          return entityList;
        //}

        //Protected Function ExecuteStoredProc(command As SqlCommand) As IEnumerable(Of T)
        //	Dim list = New List(Of T)()

        //	command.Connection = _connection
        //	command.CommandType = CommandType.StoredProcedure
        //	_connection.Open()

        //	Try
        //		Dim reader = command.ExecuteReader()
        //		Try
        //			If reader.HasRows() Then
        //				list.AddRange(MapDataToEntityCollection(reader))
        //			End If

        //		Catch ex As Exception
        //			Throw ex

        //		Finally
        //			' Always call Close when done reading.
        //			reader.Close()
        //		End Try

        //	Catch ex As Exception
        //		Throw ex

        //	Finally
        //		_connection.Close()
        //	End Try

        //	Return list
        //End Function

        //#Region "IDisposable Support"
        //		Private disposedValue As Boolean ' To detect redundant calls

        //		' IDisposable
        //		Protected Overridable Sub Dispose(disposing As Boolean)
        //			If Not Me.disposedValue Then
        //				If disposing Then
        //					' TODO: dispose managed state (managed objects).
        //					_connection.Close()
        //				End If

        //				' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
        //				' TODO: set large fields to null.
        //			End If
        //			Me.disposedValue = True
        //		End Sub

        //		' TODO: override Finalize() only if Dispose(ByVal disposing As Boolean) above has code to free unmanaged resources.
        //		'Protected Overrides Sub Finalize()
        //		'    ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
        //		'    Dispose(False)
        //		'    MyBase.Finalize()
        //		'End Sub

        //		' This code added by Visual Basic to correctly implement the disposable pattern.
        //		Public Sub Dispose() Implements IDisposable.Dispose
        //			' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
        //			Dispose(True)
        //			GC.SuppressFinalize(Me)
        //		End Sub
        //#End Region
    }
}
