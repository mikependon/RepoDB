using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.SqlServer.IntegrationTests.Setup;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.SqlServer.IntegrationTests
{
    [TestClass]
    public class AdditionalDbTypesTests
    {
        [TestInitialize]
        public void Initialize()
        {
            GlobalConfiguration
                .Setup(new()
                {
                    ConversionType = Enumerations.ConversionType.Automatic
                })
                .UseSqlServer();

            Database.Initialize();
            Cleanup();

            using var connection = new SqlConnection(Database.ConnectionString).EnsureOpen();

            connection.ExecuteNonQuery($@"
                IF (NOT EXISTS(SELECT 1 FROM [sys].[objects] WHERE type = 'U' AND name = '{nameof(DateOnlyTestData)}'))
                BEGIN
                    CREATE TABLE [dbo].[{nameof(DateOnlyTestData)}] (
                        [Id] INT IDENTITY(1, 1),
                        [DateOnly]              DATE NOT NULL,
                        [DateOnlyNullable]      DATE NULL,
                        CONSTRAINT [{nameof(DateOnlyTestData)}_Id] PRIMARY KEY 
		                (
			                [Id] ASC
		                )
                    ) ON [PRIMARY]
                END");

            // Do this again as this is now overwritten
            GlobalConfiguration
                .Setup(new()
                {
                    ConversionType = Enumerations.ConversionType.Automatic
                })
                .UseSqlServer();
        }

        [TestCleanup]
        public void Cleanup()
        {
            Database.Cleanup();
        }

        [TestMethod]
        public async Task TestDateTimeOnlyInsertQuery()
        {
            await using var connection = new SqlConnection(Database.ConnectionString);
            await connection.OpenAsync();
            await using var t = await connection.BeginTransactionAsync();

            await connection.InsertAllAsync(
                new DateOnlyTestData[] {
                    new()
                    {
                        DateOnly = new DateOnly(2024,1,1),
                        DateOnlyNullable = null,
                    },
                    new ()
                    {
                        DateOnly = new DateOnly(2025,1,1),
                        DateOnlyNullable = new DateOnly(2026,1,1),
                    }
                },
                transaction: t);


            var all = await connection.QueryAllAsync<DateOnlyTestData>(transaction: t);

            Assert.IsTrue(all.Any(x => x.DateOnly == new DateOnly(2024, 1, 1)), "Found DateOnly");
            Assert.IsTrue(all.Any(x => x.DateOnlyNullable == new DateOnly(2026, 1, 1)), "Found nullable DateOnly");
            Assert.IsTrue(all.Any(x => x.DateOnlyNullable == null), "Found null DateOnly?");

            var cmp1 = new DateOnly(2024, 1, 1);
            Assert.AreEqual(1, (await connection.QueryAsync<DateOnlyTestData>(where: x => x.DateOnly == cmp1, transaction: t)).Count());
            Assert.IsTrue((await connection.QueryAsync<DateOnlyTestData>(where: x => x.DateOnlyNullable == new DateOnly(2026, 1, 1), transaction: t)).Count() == 1);
        }



        public class DateOnlyTestData
        {
            public int ID { get; set; }
            public DateOnly DateOnly { get; set; }
            public DateOnly? DateOnlyNullable { get; set; }
        }
    }
}
