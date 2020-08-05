using System;
using Microsoft.Data.SqlClient;

namespace RepoDb.Benchmarks.SqlServer.Setup
{
    public static class DatabaseHelper
    {
        public static string ConnectionString { get; private set; }

        public static void Initialize(int elementsCount)
        {
            var connectionString = Environment.GetEnvironmentVariable("REPODB_CONSTR", EnvironmentVariableTarget.Process);

            ConnectionString = connectionString ?? @"Server=(local);Database=RepoDbTest;Integrated Security=False;User Id=michael;Password=Password123;";

            CreateDatabase();
            CreatePersonTable();
            FillData(elementsCount);
        }

        private static void FillData(int elementsCount)
        {
            const string commandText = @"DECLARE @i INT = 1;
	            WHILE @i <= @elementsCount
                BEGIN
                    INSERT INTO [dbo].[Person] (Name, Age, CreatedDateUtc) 
                    VALUES (REPLICATE('x', 128), @i, GETDATE());
                    Set @i = @i + 1;
                END";

            using var connection = new SqlConnection(ConnectionString);

            var command = new SqlCommand(commandText, connection);
            command.Parameters.AddWithValue("@elementsCount", elementsCount);

            connection.Open();
            command.ExecuteNonQuery();
        }

        public static void Cleanup()
        {
            const string commandText = "TRUNCATE TABLE [dbo].[Person];";

            using var connection = new SqlConnection(ConnectionString);

            connection.Open();
            connection.ExecuteNonQuery(commandText);
        }

        private static void CreateDatabase()
        {
            const string commandText = @"IF (NOT EXISTS(SELECT * FROM sys.databases WHERE name = 'RepoDbTest'))
                BEGIN
	                CREATE DATABASE [RepoDbTest];
                END";

            using var connection = new SqlConnection(ConnectionString);

            connection.Open();
            connection.ExecuteNonQuery(commandText);
        }

        private static void CreatePersonTable()
        {
            const string commandText = @"IF (NOT EXISTS(SELECT * FROM sys.tables WHERE name = 'Person'))
                BEGIN
                    CREATE TABLE [dbo].[Person]
                    (
	                    [Id] [bigint] IDENTITY(1,1) NOT NULL,
	                    [Name] [nvarchar](128) NOT NULL,
	                    [Age] [int] NOT NULL,
	                    [CreatedDateUtc] [datetime2](5) NOT NULL,
	                    CONSTRAINT [CRIX_Person_Id] PRIMARY KEY CLUSTERED ([Id] ASC) ON [PRIMARY]
                    )
                    ON [PRIMARY];
                END";

            using var connection = new SqlConnection(ConnectionString);

            connection.Open();
            connection.ExecuteNonQuery(commandText);
        }
    }
}