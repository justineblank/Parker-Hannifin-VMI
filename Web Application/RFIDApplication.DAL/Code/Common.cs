using System;
using System.Linq;
using System.IO;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace RFIDApplication.DAL.Code
{
    public static class Common
    {

        public static async Task<string> RunSQLQueryFromFile(string connectionString, string dbName, string filePath, string databaseVersion = "")
        {
            string errors = string.Empty;
            string scriptText = GetSQLQuery(dbName, filePath, databaseVersion);
            string[] splitter = new string[] { "GO--split" };
            string[] commandTexts = scriptText.Split(splitter, StringSplitOptions.None);

            // execute the first command - usually the "use database" or create database command
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                try
                {
                    using (SqlCommand cmd = new SqlCommand(commandTexts[0], connection))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    errors = string.Format("{0}\n{1}\n{2}", errors, ex.Message, commandTexts[0]);
                }
            }

            // execute the remainder if there are no errors
            if (string.IsNullOrEmpty(errors))
            {
                // set connection string for our database
                SqlConnectionStringBuilder sqlcnxstringbuilder = new SqlConnectionStringBuilder(connectionString);
                sqlcnxstringbuilder.InitialCatalog = dbName;
                connectionString = sqlcnxstringbuilder.ConnectionString;

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlTransaction transaction = connection.BeginTransaction())
                    {
                        // execute the remainder of the commands
                        dynamic commandTextList = commandTexts.Skip(1).Where(e => !string.IsNullOrEmpty(e.Trim())).ToList();
                        foreach (string commandText in commandTextList)
                        {
                            try
                            {
                                using (SqlCommand cmd = new SqlCommand(commandText, connection, transaction))
                                {
                                    cmd.ExecuteNonQuery();
                                }
                            }
                            catch (Exception ex)
                            {
                                errors = string.Format("{0}\n{1}\n{2}", errors, ex.Message, commandText);
                            }
                        }

                        if (string.IsNullOrEmpty(errors))
                            transaction.Commit();
                        else
                            transaction.Rollback();
                    }
                }
            }
            return errors;
        }

        private static string GetSQLQuery(string databaseName, string filePath, string databaseVersion = "")
        {
            string strContents = File.ReadAllText(filePath);
            strContents = strContents.Replace("@CompanyAlias", databaseName);
            strContents = strContents.Replace("@DatabaseCurrentVersion", ((databaseVersion == null) ? "0" : databaseVersion));
            return strContents;
        }

    }
}