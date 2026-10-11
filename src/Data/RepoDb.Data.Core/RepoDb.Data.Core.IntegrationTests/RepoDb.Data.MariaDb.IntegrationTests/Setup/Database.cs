#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using RepoDb.Connector.MariaDb;
using RepoDb.DbSettings;
using RepoDb.Schema.MariaDb;

namespace RepoDb.Data.MariaDb.IntegrationTests.Setup
{
    public static class Database
    {
        #region Properties

        /// <summary>
        /// The database of the source tables.
        /// </summary>
        public const string SourceName = "repodb_mdb_source";

        /// <summary>
        /// The database that is the Sales schema of the source tables (the schema of a table is its database in MySQL).
        /// </summary>
        public const string SourceSalesName = "repodb_mdb_source_sales";

        /// <summary>
        /// The database that receives the tables.
        /// </summary>
        public const string TargetName = "repodb_mdb_target";

        /// <summary>
        /// The database that is the Sales schema of the target tables.
        /// </summary>
        public const string TargetSalesName = "repodb_mdb_target_sales";

        /// <summary>
        /// Gets or sets the connection string to be used for the MySQL system database.
        /// </summary>
        public static string ConnectionStringForMaster { get; private set; }

        /// <summary>
        /// Gets or sets the connection string to be used for the source database (the data is read from here).
        /// </summary>
        public static string ConnectionStringForSource { get; private set; }

        /// <summary>
        /// Gets or sets the connection string to be used for the target database (the data is copied here).
        /// </summary>
        public static string ConnectionStringForTarget { get; private set; }

        #endregion

        #region Methods

        public static void Initialize()
        {
            // Master connection
            ConnectionStringForMaster =
                Environment.GetEnvironmentVariable("REPODB_MARIADB_DATA_CONSTR_MASTER") ??
                "Server=127.0.0.1;Port=3307;Database=sys;User ID=root;Password=RepoDB2026;Pooling=false;";

            // Source connection (the bulk operations load the data from a local file)
            ConnectionStringForSource =
                Environment.GetEnvironmentVariable("REPODB_MARIADB_DATA_CONSTR_SOURCE") ??
                $"Server=127.0.0.1;Port=3307;Database={SourceName};User ID=root;Password=RepoDB2026;Pooling=false;AllowLoadLocalInfile=True;AllowUserVariables=True;";

            // Target connection
            ConnectionStringForTarget =
                Environment.GetEnvironmentVariable("REPODB_MARIADB_DATA_CONSTR_TARGET") ??
                $"Server=127.0.0.1;Port=3307;Database={TargetName};User ID=root;Password=RepoDB2026;Pooling=false;AllowLoadLocalInfile=True;AllowUserVariables=True;";

            // Initialize the MySQL
            GlobalConfiguration
                .Setup()
                .UseMariaDb(new MariaDbBulkOperationsDbSetting())
                .UseMariaDbSchema();

            // The server must allow the bulk operations to load the data from a local file
            using (var connection = new MariaDbConnection(ConnectionStringForMaster).EnsureOpen())
            {
                connection.ExecuteNonQuery("SET GLOBAL local_infile = 1;");
            }

            // Create the source tables (with their rows) and reset the target
            CreateSourceTables();
            Cleanup();
        }

        public static void Cleanup()
        {
            // The target databases are dedicated to the tests, so they are dropped and created again
            using (var connection = new MariaDbConnection(ConnectionStringForMaster).EnsureOpen())
            {
                foreach (var name in new[] { TargetName, TargetSalesName })
                {
                    connection.ExecuteNonQuery($"DROP DATABASE IF EXISTS `{name}`;");
                    connection.ExecuteNonQuery($"CREATE DATABASE `{name}`;");
                }
            }

            // The fields of the tables are cached (by the connection string and the name of the table), and the tables are not the same in the next test
            DbFieldCache.Flush();
        }

        #endregion

        #region CreateTables

        private static void CreateSourceTables()
        {
            using (var master = new MariaDbConnection(ConnectionStringForMaster).EnsureOpen())
            {
                // The source tables are created (with their rows) only once: they are not changed by the tests
                var exists = master.ExecuteScalar<long>($"SELECT COUNT(*) FROM information_schema.tables WHERE table_schema = '{SourceName}' AND table_name = 'Empty';");
                if (exists > 0)
                {
                    return;
                }

                foreach (var name in new[] { SourceName, SourceSalesName })
                {
                    master.ExecuteNonQuery($"DROP DATABASE IF EXISTS `{name}`;");
                    master.ExecuteNonQuery($"CREATE DATABASE `{name}`;");
                }
            }

            using (var connection = new MariaDbConnection(ConnectionStringForSource).EnsureOpen())
            {
                // The root of the tree: Country <- Person <- Pet <- Toy
                connection.ExecuteNonQuery(@"CREATE TABLE `Country`
                    (
                        `Id` INT NOT NULL AUTO_INCREMENT,
                        `Name` VARCHAR(100) NOT NULL,
                        PRIMARY KEY (`Id`),
                        UNIQUE KEY `UQ_Country_Name` (`Name`)
                    );
                    INSERT INTO `Country` (`Name`) VALUES ('Philippines'), ('Japan'), ('Germany');");

                connection.ExecuteNonQuery(@"CREATE TABLE `Person`
                    (
                        `Id` INT NOT NULL AUTO_INCREMENT,
                        `Name` VARCHAR(128) NOT NULL,
                        `Age` INT NULL,
                        `CountryId` INT NULL,
                        `Salary` DECIMAL(18, 2) NULL,
                        `Birthday` DATE NULL,
                        `IsActive` TINYINT(1) NOT NULL DEFAULT 1,
                        `CreatedDateUtc` DATETIME(3) NOT NULL DEFAULT CURRENT_TIMESTAMP(3),
                        PRIMARY KEY (`Id`),
                        CONSTRAINT `CK_Person_Age` CHECK (`Age` >= 0),
                        CONSTRAINT `FK_Person_Country` FOREIGN KEY (`CountryId`) REFERENCES `Country` (`Id`),
                        KEY `IX_Person_Name` (`Name`)
                    );
                    INSERT INTO `Person` (`Name`, `Age`, `CountryId`, `Salary`, `Birthday`, `IsActive`, `CreatedDateUtc`) VALUES
                        ('Alice', 30, 1, 1000.50, '1990-01-15', 1, '2026-01-01 10:00:00.123'),
                        ('Bob',   45, 2, 2500.00, '1975-06-30', 1, '2026-01-02 11:00:00.456'),
                        ('Carol', NULL, NULL, NULL, NULL, 0, '2026-01-03 12:00:00.789'),
                        ('Dave',  22, 1, 800.25,  '2003-12-01', 1, '2026-01-04 13:00:00.000'),
                        ('Eve',   37, 3, 3100.75, '1988-03-09', 0, '2026-01-05 14:00:00.321'),
                        ('Frank', 52, 2, NULL,    '1970-11-22', 1, '2026-01-06 15:00:00.654');");

                connection.ExecuteNonQuery(@"CREATE TABLE `Pet`
                    (
                        `Id` INT NOT NULL AUTO_INCREMENT,
                        `Name` VARCHAR(50) NOT NULL,
                        `PersonId` INT NOT NULL,
                        PRIMARY KEY (`Id`),
                        CONSTRAINT `FK_Pet_Person` FOREIGN KEY (`PersonId`) REFERENCES `Person` (`Id`)
                    );
                    INSERT INTO `Pet` (`Name`, `PersonId`) VALUES ('Rex', 1), ('Tom', 1), ('Kiko', 2), ('Luna', 4), ('Milo', 5);");

                connection.ExecuteNonQuery(@"CREATE TABLE `Toy`
                    (
                        `Id` INT NOT NULL AUTO_INCREMENT,
                        `Name` VARCHAR(50) NOT NULL,
                        `PetId` INT NOT NULL,
                        PRIMARY KEY (`Id`),
                        CONSTRAINT `FK_Toy_Pet` FOREIGN KEY (`PetId`) REFERENCES `Pet` (`Id`)
                    );
                    INSERT INTO `Toy` (`Name`, `PetId`) VALUES ('Ball', 1), ('Bone', 1), ('Mouse', 2), ('Yarn', 3), ('Stick', 4);");

                // A table that is not related to any other table
                connection.ExecuteNonQuery(@"CREATE TABLE `Currency`
                    (
                        `Code` CHAR(3) NOT NULL,
                        `Name` VARCHAR(50) NOT NULL,
                        PRIMARY KEY (`Code`)
                    );
                    INSERT INTO `Currency` (`Code`, `Name`) VALUES ('PHP', 'Philippine Peso'), ('JPY', 'Japanese Yen'), ('EUR', 'Euro');");

                // One column of each of the common data types, and a row of NULL values
                connection.ExecuteNonQuery(@"CREATE TABLE `Sample`
                    (
                        `Id` INT NOT NULL,
                        `BigIntValue` BIGINT NULL,
                        `BitValue` TINYINT(1) NULL,
                        `DecimalValue` DECIMAL(18, 4) NULL,
                        `FloatValue` DOUBLE NULL,
                        `DateTime2Value` DATETIME(6) NULL,
                        `DateValue` DATE NULL,
                        `GuidValue` CHAR(36) NULL,
                        `BinaryValue` LONGBLOB NULL,
                        `TextValue` LONGTEXT NULL,
                        `CharValue` VARCHAR(50) NULL,
                        PRIMARY KEY (`Id`)
                    ) DEFAULT CHARSET = utf8mb4;
                    INSERT INTO `Sample` VALUES
                        (1, 9223372036854775807, 1, 12345.6789, 3.14159265358979, '2026-02-03 04:05:06.123456', '2026-02-03', '6f9619ff-8b86-d011-b42d-00c04fc964ff', 0x0102030405, 'A long text with unicode: ñ é 日本語', 'abc'),
                        (2, -9223372036854775807, 0, -0.0001, -2.5E-10, '1000-01-01 00:00:00', '9999-12-31', '00000000-0000-0000-0000-000000000000', X'', '', ''),
                        (3, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);");

                // A table whose name and column have a space
                connection.ExecuteNonQuery(@"CREATE TABLE `Order Details`
                    (
                        `Id` INT NOT NULL,
                        `Unit Price` DECIMAL(10, 2) NOT NULL,
                        PRIMARY KEY (`Id`)
                    );
                    INSERT INTO `Order Details` (`Id`, `Unit Price`) VALUES (1, 9.99), (2, 19.50);");

                // A table with many rows, to copy the rows in many batches
                connection.ExecuteNonQuery(@"CREATE TABLE `Number`
                    (
                        `Value` INT NOT NULL,
                        `Label` VARCHAR(20) NOT NULL,
                        PRIMARY KEY (`Value`)
                    );");
                connection.ExecuteNonQuery(@"SET SESSION cte_max_recursion_depth = 3000;
                    INSERT INTO `Number` (`Value`, `Label`)
                        WITH RECURSIVE `Numbers` (`N`) AS (SELECT 1 UNION ALL SELECT `N` + 1 FROM `Numbers` WHERE `N` < 2500)
                        SELECT `N`, CONCAT('Number ', `N`) FROM `Numbers`;");

                // A self-referencing foreign key
                connection.ExecuteNonQuery(@"CREATE TABLE `Employee`
                    (
                        `Id` INT NOT NULL,
                        `Name` VARCHAR(50) NOT NULL,
                        `ManagerId` INT NULL,
                        PRIMARY KEY (`Id`),
                        CONSTRAINT `FK_Employee_Manager` FOREIGN KEY (`ManagerId`) REFERENCES `Employee` (`Id`)
                    );
                    INSERT INTO `Employee` (`Id`, `Name`, `ManagerId`) VALUES (1, 'CEO', NULL), (2, 'VP', 1), (3, 'Developer', 2);");

                // A cycle of 2 tables
                connection.ExecuteNonQuery(@"CREATE TABLE `CycleA`
                    (
                        `Id` INT NOT NULL,
                        `BId` INT NULL,
                        PRIMARY KEY (`Id`)
                    );
                    CREATE TABLE `CycleB`
                    (
                        `Id` INT NOT NULL,
                        `AId` INT NULL,
                        PRIMARY KEY (`Id`),
                        CONSTRAINT `FK_CycleB_CycleA` FOREIGN KEY (`AId`) REFERENCES `CycleA` (`Id`)
                    );
                    ALTER TABLE `CycleA` ADD CONSTRAINT `FK_CycleA_CycleB` FOREIGN KEY (`BId`) REFERENCES `CycleB` (`Id`);
                    INSERT INTO `CycleA` (`Id`, `BId`) VALUES (1, NULL);
                    INSERT INTO `CycleB` (`Id`, `AId`) VALUES (1, 1);
                    UPDATE `CycleA` SET `BId` = 1 WHERE `Id` = 1;");

                // A table without a primary key
                connection.ExecuteNonQuery(@"CREATE TABLE `NoKey`
                    (
                        `Value` VARCHAR(50) NULL,
                        `Amount` INT NULL
                    );
                    INSERT INTO `NoKey` (`Value`, `Amount`) VALUES ('A', 1), ('A', 1), (NULL, NULL);");

                // Tables of the Sales schema (another database), and a table of the default schema that references it
                connection.ExecuteNonQuery($@"CREATE TABLE `{SourceSalesName}`.`Invoice`
                    (
                        `Id` INT NOT NULL,
                        `Total` DECIMAL(19, 4) NOT NULL,
                        PRIMARY KEY (`Id`)
                    );
                    CREATE TABLE `{SourceSalesName}`.`InvoiceLine`
                    (
                        `Id` INT NOT NULL,
                        `InvoiceId` INT NOT NULL,
                        `Description` VARCHAR(100) NOT NULL,
                        PRIMARY KEY (`Id`),
                        CONSTRAINT `FK_InvoiceLine_Invoice` FOREIGN KEY (`InvoiceId`) REFERENCES `{SourceSalesName}`.`Invoice` (`Id`)
                    );
                    CREATE TABLE `Ledger`
                    (
                        `Id` INT NOT NULL,
                        `InvoiceId` INT NOT NULL,
                        PRIMARY KEY (`Id`),
                        CONSTRAINT `FK_Ledger_Invoice` FOREIGN KEY (`InvoiceId`) REFERENCES `{SourceSalesName}`.`Invoice` (`Id`)
                    );
                    INSERT INTO `{SourceSalesName}`.`Invoice` (`Id`, `Total`) VALUES (1, 100.0000), (2, 250.5000);
                    INSERT INTO `{SourceSalesName}`.`InvoiceLine` (`Id`, `InvoiceId`, `Description`) VALUES (1, 1, 'Line 1'), (2, 1, 'Line 2'), (3, 2, 'Line 3');
                    INSERT INTO `Ledger` (`Id`, `InvoiceId`) VALUES (1, 1), (2, 2);");

                // A table without rows (it is the last one: it tells that the source tables are all created)
                connection.ExecuteNonQuery(@"CREATE TABLE `Empty`
                    (
                        `Id` INT NOT NULL,
                        `Name` VARCHAR(50) NULL,
                        PRIMARY KEY (`Id`)
                    );");
            }
        }

        #endregion
    }
}
