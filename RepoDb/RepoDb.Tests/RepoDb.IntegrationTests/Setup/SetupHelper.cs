using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using RepoDb.Extensions;

namespace RepoDb.IntegrationTests.Setup {
    public static class Constants
    {
        public static readonly string TestDatabase = @"Server=.;Database=REPODBTST;Integrated Security=True;";
    }

    public static class SetupHelper
    {
        public static void InitDatabase()
        {
            string script;
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "RepoDb.IntegrationTests.Setup.SetupDB.sql";

            using (var stream = assembly.GetManifestResourceStream(resourceName))
            using (var reader = new StreamReader(stream ?? throw new InvalidOperationException()))
            {
                script = reader.ReadToEnd();
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

        public static void CleanDatabase()
        {
            using (var connection = new SqlConnection(Constants.TestDatabase))
            {
                connection.ExecuteNonQuery("DELETE FROM [dbo].[OrderDetail];");
                connection.ExecuteNonQuery("DELETE FROM [dbo].[Order];");
                connection.ExecuteNonQuery("DELETE FROM [dbo].[Customer];");

                connection.ExecuteNonQuery("DBCC CHECKIDENT ([OrderDetail], RESEED, 1);");
                connection.ExecuteNonQuery("DBCC CHECKIDENT ([Order], RESEED, 1);");
                connection.ExecuteNonQuery("DBCC CHECKIDENT ([Customer], RESEED, 1);");
            }
        }
    }
}
