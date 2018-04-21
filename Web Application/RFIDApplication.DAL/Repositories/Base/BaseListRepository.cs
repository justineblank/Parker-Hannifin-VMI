using Microsoft.Extensions.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Reflection;
using System.Threading.Tasks;
using RFIDApplication.DAL.Interfaces;

namespace RFIDApplication.DAL.Repositories
{
    public abstract class BaseListRepository : BaseRepository
	{
		public BaseListRepository(IConfigurationRoot config, 
                                ICurrentPrincipalAccessor currentPrincipalAccessor, 
                                IUnauthenticatedHttpContextAccessor unauthenticatedHttpContextAccessor)
            : base(config, currentPrincipalAccessor, unauthenticatedHttpContextAccessor)
        {
		}

        protected async Task<List<ModelType>> MapDataToModelListAsync<ModelType>(DbDataReader dr) where ModelType : new()
        {
            Type entityType = typeof(ModelType);
            List<ModelType> entityList = new List<ModelType>();
            Hashtable hashtable = new Hashtable();
            PropertyInfo[] properties = entityType.GetProperties();

            try
            {
                foreach (PropertyInfo info in properties)
                {
                    hashtable[info.Name.ToUpper()] = info;
                }

                while (await dr.ReadAsync())
                {
                    ModelType newObject = new ModelType();
                    for (int index = 0; index <= dr.FieldCount - 1; index++)
                    {
                        PropertyInfo info = (PropertyInfo)hashtable[dr.GetName(index).Substring(3).ToUpper()];
                        if ((info != null) && info.CanWrite && (dr.GetValue(index) != DBNull.Value))
                        {
                            Type fieldType = dr.GetFieldType(index);

                            if (fieldType.Name == "DateTime")
                            {
                                DateTime dt = (DateTime)dr.GetValue(index);
                                info.SetValue(newObject, dt.AddMinutes(-1 * this.TimeZoneOffset), null);
                            }
                            else
                            {
                                info.SetValue(newObject, dr.GetValue(index), null);
                            }
                        }
                    }
                    entityList.Add(newObject);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dr.Dispose();
            }

            return entityList;
        }
	}
}
