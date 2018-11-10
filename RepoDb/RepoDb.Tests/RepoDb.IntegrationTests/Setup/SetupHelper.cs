using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace RepoDb.IntegrationTests.Setup
{
    public static class SetupHelper
    {
        public static void ExecuteEmbeddedSqlFile(string resourceName)
        {
            string script;
            var assembly = Assembly.GetExecutingAssembly();
            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                using (var reader = new StreamReader(stream))
                {
                    script = reader.ReadToEnd();
                }
            }

            using (var connection = new SqlConnection(Constants.TestDatabase))
            {
                var sqlStatements = Regex.Split(script, @"^\s*GO\s*$", RegexOptions.Multiline | RegexOptions.IgnoreCase);
                foreach (var sqlStatement in sqlStatements)
                {
                    Debug.WriteLine($"Executing sql statement{Environment.NewLine}{sqlStatement}");
                    if (sqlStatement.Trim() != "")
                    {
                        var rowsAffected = connection.ExecuteNonQuery(sqlStatement);
                        Debug.WriteLine($"Affected {rowsAffected} rows.");
                    }
                }
            }
        }

        public static void InitDatabase()
        {
            var resourceName = "RepoDb.IntegrationTests.Setup.SetupDB.sql";
            ExecuteEmbeddedSqlFile(resourceName);
        }

        public static void CleanDatabase()
        {
            using (var connection = new SqlConnection(Constants.TestDatabase))
            {
                connection.ExecuteNonQuery("DELETE FROM [dbo].[OrderDetail];");
                connection.ExecuteNonQuery("DELETE FROM [dbo].[Order];");
                connection.ExecuteNonQuery("DELETE FROM [dbo].[Customer];");
                connection.ExecuteNonQuery("DELETE FROM [dbo].[TypeMap];");
                connection.ExecuteNonQuery("DBCC CHECKIDENT ([OrderDetail], RESEED, 1);");
                connection.ExecuteNonQuery("DBCC CHECKIDENT ([Order], RESEED, 1);");
                connection.ExecuteNonQuery("DBCC CHECKIDENT ([Customer], RESEED, 1);");
            }
        }
    }
}
