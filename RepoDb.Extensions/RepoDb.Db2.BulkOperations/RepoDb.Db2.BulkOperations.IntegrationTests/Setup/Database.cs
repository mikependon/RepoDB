using IBM.Data.Db2;
using RepoDb.Db2.BulkOperations.IntegrationTests.Models;
using RepoDb.Db2.PropertyHandlers;
using System;
using System.Linq;

namespace RepoDb.IntegrationTests.Setup
{
    /// <summary>
    /// A class used as a startup setup for the RepoDb Db2 bulk-operations test database.
    /// </summary>
    public static class Database
    {
        #region Properties

        /// <summary>
        /// Gets the connection string to be used for the Db2 database.
        /// </summary>
        public static string ConnectionString { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Initialize the creation of the database.
        /// </summary>
        public static void Initialize()
        {
            // Defaults match the 'db2' service in docker-compose.yml at the repo root (a Db2
            // community/Db2Connect image, DB2INST1_PASSWORD=RepoDB2026), which exposes the
            // "db2inst1" user against the "REPODB" database on the default port 50000. Same
            // connection string shape (and env var name) as RepoDb.Db2.IntegrationTests -
            // "HostVarParameters=True;" is required for the same reason documented there: every
            // RepoDb-generated statement binds using ":Name"-style host variables, which
            // IBM.Data.Db2 doesn't recognize by default.
            ConnectionString =
                Environment.GetEnvironmentVariable("REPODB_Db2_CONSTR") ??
                "Server=localhost:50000;Database=REPODB;UID=db2inst1;PWD=RepoDB2026;HostVarParameters=True;";

            // Initialize Db2
            GlobalConfiguration
                .Setup()
                .UseDb2();

            // Db2 has no native GUID type; "RowGuid"/"RowGuidMapped" are stored as 16-byte
            // "CHAR(16) FOR BIT DATA" columns. Scoped per data-entity type (rather than registered
            // globally for typeof(Guid)) since a global registration would also affect any other
            // provider used in the same process - each of the six CLR types below maps onto one of
            // the two physical tables, so each needs its own registration even though the
            // underlying column shape is shared. Same pattern as RepoDb.Db2.IntegrationTests.
            PropertyHandlerMapper.Add<BulkOperationIdentityTable, Db2GuidToByteArrayPropertyHandler>(
                e => e.RowGuid, new Db2GuidToByteArrayPropertyHandler(), true);
            PropertyHandlerMapper.Add<BulkOperationNonIdentityTable, Db2GuidToByteArrayPropertyHandler>(
                e => e.RowGuid, new Db2GuidToByteArrayPropertyHandler(), true);
            PropertyHandlerMapper.Add<WithExtraFieldsBulkOperationIdentityTable, Db2GuidToByteArrayPropertyHandler>(
                e => e.RowGuid, new Db2GuidToByteArrayPropertyHandler(), true);
            PropertyHandlerMapper.Add<WithExtraFieldsBulkOperationNonIdentityTable, Db2GuidToByteArrayPropertyHandler>(
                e => e.RowGuid, new Db2GuidToByteArrayPropertyHandler(), true);
            PropertyHandlerMapper.Add<BulkOperationMappedIdentityTable, Db2GuidToByteArrayPropertyHandler>(
                e => e.RowGuidMapped, new Db2GuidToByteArrayPropertyHandler(), true);
            PropertyHandlerMapper.Add<BulkOperationMappedNonIdentityTable, Db2GuidToByteArrayPropertyHandler>(
                e => e.RowGuidMapped, new Db2GuidToByteArrayPropertyHandler(), true);

            // Db2 has no native TINYINT type; "ColumnBit"/"ColumnBitMapped" are stored as SMALLINT
            // columns. The IBM Data Server .NET Provider doesn't marshal a raw, boxed System.Byte
            // parameter value cleanly against it - convert to System.Int16 first, same scoping
            // rationale as the Guid<->byte[] handler above. Same pattern as RepoDb.Db2.IntegrationTests.
            PropertyHandlerMapper.Add<BulkOperationIdentityTable, Db2ByteToInt16PropertyHandler>(
                e => e.ColumnBit, new Db2ByteToInt16PropertyHandler(), true);
            PropertyHandlerMapper.Add<BulkOperationNonIdentityTable, Db2ByteToInt16PropertyHandler>(
                e => e.ColumnBit, new Db2ByteToInt16PropertyHandler(), true);
            PropertyHandlerMapper.Add<WithExtraFieldsBulkOperationIdentityTable, Db2ByteToInt16PropertyHandler>(
                e => e.ColumnBit, new Db2ByteToInt16PropertyHandler(), true);
            PropertyHandlerMapper.Add<WithExtraFieldsBulkOperationNonIdentityTable, Db2ByteToInt16PropertyHandler>(
                e => e.ColumnBit, new Db2ByteToInt16PropertyHandler(), true);
            PropertyHandlerMapper.Add<BulkOperationMappedIdentityTable, Db2ByteToInt16PropertyHandler>(
                e => e.ColumnBitMapped, new Db2ByteToInt16PropertyHandler(), true);
            PropertyHandlerMapper.Add<BulkOperationMappedNonIdentityTable, Db2ByteToInt16PropertyHandler>(
                e => e.ColumnBitMapped, new Db2ByteToInt16PropertyHandler(), true);

            // Create the tables
            CreateTables();
        }

        /// <summary>
        /// Clean up all the table.
        /// </summary>
        public static void Cleanup()
        {
            using var connection = new DB2Connection(ConnectionString);
            connection.Truncate<BulkOperationIdentityTable>();
            connection.Truncate<BulkOperationNonIdentityTable>();
        }

        #endregion

        #region CreateTables

        /// <summary>
        /// Create the necessary tables for testing.
        /// </summary>
        public static void CreateTables()
        {
            CreateBulkOperationIdentityTable();
            CreateBulkOperationNonIdentityTable();
        }

        /// <summary>
        /// Creates an identity table that has some important fields. All fields are nullable except
        /// <c>Id</c> and <c>RowGuid</c>. <c>Id</c> is a <c>BIGINT GENERATED BY DEFAULT AS IDENTITY</c>
        /// column (a caller-supplied value, if any, still wins - "BY DEFAULT" rather than "ALWAYS" -
        /// matching how the core RepoDb.Db2 provider's own CompleteTable is set up); <c>RowGuid</c> is
        /// a <c>CHAR(16) FOR BIT DATA</c> column round-tripped via <see cref="Db2GuidToByteArrayPropertyHandler"/>
        /// (registered per-entity-type in <see cref="Initialize"/>) since Db2 has no native GUID type.
        /// </summary>
        public static void CreateBulkOperationIdentityTable()
        {
            var commandText = @"
                CREATE TABLE ""BulkOperationIdentityTable""
                (
                    ""Id"" BIGINT GENERATED BY DEFAULT AS IDENTITY (START WITH 1, INCREMENT BY 1) NOT NULL,
                    ""RowGuid"" CHAR(16) FOR BIT DATA NOT NULL,
                    ""ColumnBit"" SMALLINT NULL,
                    ""ColumnDateTime"" TIMESTAMP(0) NULL,
                    ""ColumnDateTime2"" TIMESTAMP(6) NULL,
                    ""ColumnDecimal"" DECIMAL(18,2) NULL,
                    ""ColumnFloat"" DOUBLE NULL,
                    ""ColumnInt"" INTEGER NULL,
                    ""ColumnNVarChar"" VARCHAR(2000) NULL,
                    CONSTRAINT ""BulkOperationIdentityTable_pk"" PRIMARY KEY (""Id"")
                )";
            using var connection = new DB2Connection(ConnectionString);
            ExecuteCreateTableIfNotExists(connection, commandText);
        }

        /// <summary>
        /// Creates a non-identity table that has some important fields. All fields are nullable except
        /// the primary key. Unlike <see cref="CreateBulkOperationIdentityTable"/>, <c>Id</c> here is a
        /// plain <c>BIGINT</c> primary key (no <c>IDENTITY</c> clause) - the caller's value is always
        /// stored as-is. Used by tests that need to know a row's <c>Id</c> ahead of time (e.g. matching
        /// against a separately-built anonymous/expando object by primary key), which a Db2-generated
        /// identity value can't support.
        /// </summary>
        public static void CreateBulkOperationNonIdentityTable()
        {
            var commandText = @"
                CREATE TABLE ""BulkOperationNonIdentityTable""
                (
                    ""Id"" BIGINT NOT NULL,
                    ""RowGuid"" CHAR(16) FOR BIT DATA NOT NULL,
                    ""ColumnBit"" SMALLINT NULL,
                    ""ColumnDateTime"" TIMESTAMP(0) NULL,
                    ""ColumnDateTime2"" TIMESTAMP(6) NULL,
                    ""ColumnDecimal"" DECIMAL(18,2) NULL,
                    ""ColumnFloat"" DOUBLE NULL,
                    ""ColumnInt"" INTEGER NULL,
                    ""ColumnNVarChar"" VARCHAR(2000) NULL,
                    CONSTRAINT ""BulkOperationNonIdentityTable_pk"" PRIMARY KEY (""Id"")
                )";
            using var connection = new DB2Connection(ConnectionString);
            ExecuteCreateTableIfNotExists(connection, commandText);
        }

        /// <summary>
        /// Db2 has no "CREATE TABLE IF NOT EXISTS" clause, unlike MySQL - guard against re-running
        /// this against an already-initialized database by catching SQL0601N ("the name of the
        /// object to be created is identical to the existing name of the same object type") instead.
        /// Same pattern as RepoDb.Db2.IntegrationTests/Setup/Database.cs.
        /// </summary>
        private static void ExecuteCreateTableIfNotExists(DB2Connection connection,
            string commandText)
        {
            const int ObjectAlreadyExistsSqlCode = -601; // SQL0601N/-601
            try
            {
                connection.ExecuteNonQuery(commandText);
            }
            catch (DB2Exception ex) when (ex.Errors?.Cast<DB2Error>().Any(e => e.NativeError == ObjectAlreadyExistsSqlCode) == true)
            {
                // Object already exists from a prior run - nothing to do.
            }
        }

        #endregion
    }
}
