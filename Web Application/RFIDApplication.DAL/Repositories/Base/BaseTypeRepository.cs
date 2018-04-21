using Microsoft.Extensions.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Dynamic;
using System.Reflection;
using System.Linq;
using System.Threading.Tasks;
using RFIDApplication.DAL.Interfaces;

namespace RFIDApplication.DAL.Repositories
{
    public abstract class BaseTypeRepository<T> : BaseRepository where T : new()
    {
        public BaseTypeRepository(IConfigurationRoot config,
                                ICurrentPrincipalAccessor currentPrincipalAccessor,
                                IUnauthenticatedHttpContextAccessor unauthenticatedHttpContextAccessor)
            : base(config, currentPrincipalAccessor, unauthenticatedHttpContextAccessor)
        {
        }

        protected async Task<T> MapDataToModelAsync(DbDataReader dr)
        {
            Type entityType = typeof(T);

            // JM - changed 2017-10-18 - want funtion to return null instead of new empty object
            //T entity = new T();
            T entity = default(T);

            Hashtable hashtable = new Hashtable();
            PropertyInfo[] properties = entityType.GetProperties();

            try
            {
                if (dr.HasRows)
                {
                    entity = await GetObject<T>(properties, dr);
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

            return entity;
        }

        protected async Task<ModelType> MapDataToModelAsync<ModelType>(DbDataReader dr) where ModelType : new()
        {
            Type entityType = typeof(ModelType);

            // JM - changed 2017-10-18 - want funtion to return null instead of new empty object
            //ModelType entity = new ModelType();
            ModelType entity = default(ModelType);

            PropertyInfo[] properties = entityType.GetProperties();

            try
            {
                if (dr.HasRows)
                {
                    entity = await GetObject<ModelType>(properties, dr);
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

            return entity;
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
                        PropertyInfo info = (PropertyInfo)hashtable[dr.GetName(index).ToUpper()];
                        if ((info != null) && info.CanWrite && (dr.GetValue(index) != DBNull.Value))
                        {
                            //info.SetValue(newObject, dr.GetValue(index), null);
                            Type fieldType = dr.GetFieldType(index);

                            if (fieldType.Name != "DateTime" && fieldType.Name != "Byte[]")
                            {
                                info.SetValue(newObject, dr.GetValue(index), null);
                            }
                            else
                            {
                                if (fieldType.Name == "DateTime")
                                {
                                    ////new System.Collections.Generic.Mscorlib_CollectionDebugView<System.Reflection.CustomAttributeData>(properties[3].CustomAttributes).Items[0]
                                    ////
                                    DateTime dt = (DateTime)dr.GetValue(index);
                                    //List<CustomAttributeData> lst = info.CustomAttributes.ToList();
                                    //if (lst.Count > 0
                                    //    && lst[0].AttributeType == typeof(System.ComponentModel.DataAnnotations.DisplayFormatAttribute)
                                    //    && lst[0].NamedArguments.Count > 0
                                    //    )
                                    //{
                                    //    foreach (CustomAttributeNamedArgument arg in lst[0].NamedArguments)
                                    //    {
                                    //        if (arg.MemberName.ToUpper() == "DataFormatString".ToUpper())
                                    //        {
                                    //            info.SetValue(newObject, dt.ToString(arg.TypedValue.Value.ToString().TrimStart('{').TrimEnd('}')), null);
                                    //        }
                                    //    }
                                    //    //System.ComponentModel.DataAnnotations.DisplayAttribute obj_DA = (System.ComponentModel.DataAnnotations.DisplayAttribute)lst[0];
                                    //}
                                    //else
                                    //{
                                    //dt = dt.AddMinutes(-1 * this.TimeZoneOffset);                                    
                                    info.SetValue(newObject, dt, null);
                                    //}
                                }
                                else if (fieldType.Name == "Byte[]")
                                {

                                    Byte[] bytes = (Byte[])dr.GetValue(index);

                                    PropertyInfo stringInfo = (PropertyInfo)hashtable[dr.GetName(index).ToUpper() + "STRING"];
                                    if (stringInfo != null && stringInfo.CanWrite)
                                    {
                                        stringInfo.SetValue(newObject, "data:image/gif;base64," + Convert.ToBase64String(bytes, 0, bytes.Length), null);
                                    }
                                    else
                                    {
                                        info.SetValue(newObject, bytes, null);
                                    }
                                }
                            }

                            //if (fieldType.Name == "DateTime")
                            //{
                            //    DateTime dt = (DateTime)dr.GetValue(index);
                            //    info.SetValue(newObject, dt.AddMinutes(-1 * this.TimeZoneOffset), null);
                            //}
                            //else if (fieldType.Name == "Byte[]")
                            //{
                            //    Byte[] bytes =  (Byte[])dr.GetValue(index);
                            //    info.SetValue(newObject, dt.AddMinutes(-1 * this.TimeZoneOffset), null);
                            //}
                            //else
                            //{
                            //    info.SetValue(newObject, dr.GetValue(index), null);
                            //}
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

        protected async Task<List<ModelType>> MapDataToDataRowModelListAsync<ModelType>(DbDataReader dr) where ModelType : new()
        {
            Type entityType = typeof(ModelType);
            List<ModelType> entityList = new List<ModelType>();
            Hashtable hashtable = new Hashtable();
            PropertyInfo[] properties = entityType.GetProperties();
            //var dataRow = new ExpandoObject() as IDictionary<string, object>;
            IDictionary<string, object> dataRow;// = new Dictionary<string, object>();
            try
            {

                foreach (PropertyInfo info in properties)
                {
                    hashtable[0] = info;
                }

                while (await dr.ReadAsync())
                {
                    ModelType newObject = new ModelType();
                    //PropertyInfo info = (PropertyInfo)hashtable[dr.GetName(index).ToUpper()];
                    PropertyInfo info = (PropertyInfo)hashtable[0];
                    //
                    dataRow = new Dictionary<string, object>();
                    //
                    for (int index = 0; index <= dr.FieldCount - 1; index++)
                    {
                        if ((dr.GetValue(index) != DBNull.Value)) //(info != null) && info.CanWrite &&
                        {
                            //info.SetValue(newObject, dr.GetValue(index), null);
                            Type fieldType = dr.GetFieldType(index);

                            if (fieldType.Name != "DateTime" && fieldType.Name != "Byte[]")
                            {
                                //info.SetValue(newObject, dr.GetValue(index), null);
                                dataRow.Add(
                                dr.GetName(index),
                                dr.GetValue(index));
                            }
                            else
                            {
                                if (fieldType.Name == "DateTime")
                                {
                                    DateTime dt = (DateTime)dr.GetValue(index);
                                    //info.SetValue(newObject, dt.AddMinutes(-1 * this.TimeZoneOffset), null);
                                    dataRow.Add(
                                        dr.GetName(index),
                                        dt.AddMinutes(-1 * this.TimeZoneOffset));
                                }
                                else if (fieldType.Name == "Byte[]")
                                {

                                    Byte[] bytes = (Byte[])dr.GetValue(index);

                                    PropertyInfo stringInfo = (PropertyInfo)hashtable[dr.GetName(index).ToUpper() + "STRING"];
                                    if (stringInfo != null && stringInfo.CanWrite)
                                    {
                                        //stringInfo.SetValue(newObject, "data:image/gif;base64," + Convert.ToBase64String(bytes, 0, bytes.Length), null);
                                        dataRow.Add(
                                        dr.GetName(index),
                                        "data:image/gif;base64," + Convert.ToBase64String(bytes, 0, bytes.Length));
                                    }
                                    else
                                    {
                                        //info.SetValue(newObject, bytes, null);
                                        dataRow.Add(
                                       dr.GetName(index),
                                       bytes);
                                    }
                                }
                            }
                        }
                        else
                        {
                            dataRow.Add(
                                dr.GetName(index),
                                "");
                        }
                    }
                    info.SetValue(newObject, dataRow, null);
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

        #region Private Functions
        private async Task<ModelType> GetObject<ModelType>(PropertyInfo[] properties, DbDataReader dr) where ModelType : new()
        {
            Hashtable hashtable = new Hashtable();
            ModelType newObject = new ModelType();

            foreach (PropertyInfo info in properties)
            {
                hashtable[info.Name.ToUpper()] = info;
            }

            if (await dr.ReadAsync())
            {

                for (int index = 0; index <= dr.FieldCount - 1; index++)
                {
                    PropertyInfo info = (PropertyInfo)hashtable[dr.GetName(index).ToUpper()];
                    if ((info != null) && info.CanWrite && (dr.GetValue(index) != DBNull.Value))
                    {
                        //info.SetValue(newObject, dr.GetValue(index), null);
                        Type fieldType = dr.GetFieldType(index);

                        if (fieldType.Name == "DateTime")
                        {
                            DateTime dt = (DateTime)dr.GetValue(index);
                            //dt = dt.AddMinutes(-1 * this.TimeZoneOffset);
                            info.SetValue(newObject, dt, null);
                        }
                        else
                        {
                            info.SetValue(newObject, dr.GetValue(index), null);
                        }
                    }
                }
            }
            return newObject;
        }
        #endregion
    }
}
