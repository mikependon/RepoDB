#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Data;
using MySql.Data.MySqlClient;

namespace RepoDb.Schema.MySql.IntegrationTests.Setup
{
    public static class Database
    {
        #region Properties

        /// <summary>
        /// Gets the name of the source database (the schema is read from here).
        /// </summary>
        public const string SourceName = "repodb_schema_source";

        /// <summary>
        /// Gets the name of the target database (the schema is created here).
        /// </summary>
        public const string TargetName = "repodb_schema_target";

        /// <summary>
        /// Gets the name of the other database of the source (a database is the schema of MySQL, so it plays the role of a non-default schema).
        /// </summary>
        public const string SourceSalesName = "repodb_schema_source_sales";

        /// <summary>
        /// Gets the name of the other database of the target (it plays the role of a non-default schema).
        /// </summary>
        public const string TargetSalesName = "repodb_schema_target_sales";

        /// <summary>
        /// Gets or sets the connection string to be used for the MySQL system database.
        /// </summary>
        public static string ConnectionStringForMaster { get; private set; }

        /// <summary>
        /// Gets or sets the connection string to be used for the source database (the schema is read from here).
        /// </summary>
        public static string ConnectionStringForSource { get; private set; }

        /// <summary>
        /// Gets or sets the connection string to be used for the target database (the schema is created here).
        /// </summary>
        public static string ConnectionStringForTarget { get; private set; }

        #endregion

        #region Methods

        public static void Initialize()
        {
            // Master connection
            ConnectionStringForMaster =
                Environment.GetEnvironmentVariable("REPODB_MYSQL_SCHEMA_CONSTR_MASTER") ??
                "Server=127.0.0.1;Port=3306;Database=sys;User ID=root;Password=RepoDB2026;Pooling=false;";

            // Source connection
            ConnectionStringForSource =
                Environment.GetEnvironmentVariable("REPODB_MYSQL_SCHEMA_CONSTR_SOURCE") ??
                $"Server=127.0.0.1;Port=3306;Database={SourceName};User ID=root;Password=RepoDB2026;Pooling=false;";

            // Target connection
            ConnectionStringForTarget =
                Environment.GetEnvironmentVariable("REPODB_MYSQL_SCHEMA_CONSTR_TARGET") ??
                $"Server=127.0.0.1;Port=3306;Database={TargetName};User ID=root;Password=RepoDB2026;Pooling=false;";

            // Initialize the MySQL
            GlobalConfiguration
                .Setup()
                .UseMySqlSchema();

            // Create the source tables and the target databases
            CreateSourceTables();
            Cleanup();
        }

        public static void Cleanup()
        {
            // The target databases are dedicated to the tests, so they are dropped and created again
            using (var connection = new MySqlConnection(ConnectionStringForMaster).EnsureOpen())
            {
                foreach (var name in new[] { TargetName, TargetSalesName })
                {
                    connection.ExecuteNonQuery($"DROP DATABASE IF EXISTS `{name}`;");
                    connection.ExecuteNonQuery($"CREATE DATABASE `{name}`;");
                }
            }
        }

        #endregion

        #region Helpers

        private static bool _sourceTablesCreated;

        private static void CreateSourceTables()
        {
            if (_sourceTablesCreated)
            {
                return;
            }

            using (var connection = new MySqlConnection(ConnectionStringForMaster).EnsureOpen())
            {
                foreach (var name in new[] { SourceName, SourceSalesName })
                {
                    connection.ExecuteNonQuery($"DROP DATABASE IF EXISTS `{name}`;");
                    connection.ExecuteNonQuery($"CREATE DATABASE `{name}`;");
                }
            }

            using (var connection = new MySqlConnection(ConnectionStringForSource).EnsureOpen())
            {
                connection.ExecuteNonQuery(@"
                    CREATE TABLE `Country` (
                        `Id` INT NOT NULL AUTO_INCREMENT,
                        `Name` VARCHAR(100) NOT NULL,
                        PRIMARY KEY (`Id`),
                        UNIQUE KEY `UQ_Country_Name` (`Name`)
                    );

                    CREATE TABLE `Person` (
                        `Id` BIGINT NOT NULL AUTO_INCREMENT,
                        `Name` VARCHAR(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL COMMENT 'The name of the person.',
                        `NameUpper` VARCHAR(128) GENERATED ALWAYS AS (UPPER(`Name`)) STORED,
                        `Age` INT NULL DEFAULT 0,
                        `CountryId` INT NULL,
                        `Salary` DECIMAL(18, 2) NULL,
                        `CreatedDateUtc` DATETIME(3) NOT NULL DEFAULT CURRENT_TIMESTAMP(3),
                        PRIMARY KEY (`Id`),
                        CONSTRAINT `CK_Person_Age` CHECK (`Age` >= 0),
                        CONSTRAINT `FK_Person_Country` FOREIGN KEY (`CountryId`) REFERENCES `Country` (`Id`) ON DELETE SET NULL ON UPDATE CASCADE
                    );
                    CREATE INDEX `IX_Person_Name` ON `Person` (`Name` DESC, `Age`);

                    CREATE TABLE `OrderLine` (
                        `OrderId` INT NOT NULL,
                        `LineNumber` INT NOT NULL,
                        `Quantity` INT NOT NULL,
                        PRIMARY KEY (`OrderId`, `LineNumber`)
                    );

                    CREATE TABLE `NoKey` (
                        `Value` LONGTEXT NULL,
                        `Payload` LONGBLOB NULL
                    );");

                CreateSalesTables(connection);
                CreateRelationshipTables(connection);
                CreateIndexTables(connection);
                CreateOddNameTables(connection);
            }

            _sourceTablesCreated = true;
        }

        private static void CreateSalesTables(IDbConnection connection) =>
            connection.ExecuteNonQuery($@"
                -- A database is the schema of MySQL, so the other database plays the role of a non-default schema
                CREATE TABLE `{SourceSalesName}`.`Invoice` (
                    `Id` INT NOT NULL AUTO_INCREMENT,
                    `Total` DECIMAL(19, 4) NOT NULL,
                    PRIMARY KEY (`Id`)
                );

                -- A foreign key within the other database, and a foreign key across the databases
                CREATE TABLE `{SourceSalesName}`.`InvoiceLine` (
                    `Id` INT NOT NULL,
                    `InvoiceId` INT NOT NULL,
                    PRIMARY KEY (`Id`),
                    CONSTRAINT `FK_InvoiceLine_Invoice` FOREIGN KEY (`InvoiceId`) REFERENCES `{SourceSalesName}`.`Invoice` (`Id`)
                );
                CREATE TABLE `Ledger` (
                    `Id` INT NOT NULL,
                    `InvoiceId` INT NOT NULL,
                    PRIMARY KEY (`Id`),
                    CONSTRAINT `FK_Ledger_Invoice` FOREIGN KEY (`InvoiceId`) REFERENCES `{SourceSalesName}`.`Invoice` (`Id`) ON DELETE CASCADE
                );

                -- 2 tables with the same name in different databases, and a table that references the one in the other database
                CREATE TABLE `Item` (
                    `Id` INT NOT NULL,
                    PRIMARY KEY (`Id`)
                );
                CREATE TABLE `{SourceSalesName}`.`Item` (
                    `Id` INT NOT NULL,
                    PRIMARY KEY (`Id`)
                );
                CREATE TABLE `ItemRef` (
                    `Id` INT NOT NULL,
                    `ItemId` INT NOT NULL,
                    PRIMARY KEY (`Id`),
                    CONSTRAINT `FK_ItemRef_Item` FOREIGN KEY (`ItemId`) REFERENCES `{SourceSalesName}`.`Item` (`Id`)
                );");

        private static void CreateRelationshipTables(IDbConnection connection) =>
            connection.ExecuteNonQuery(@"
                -- A dependency chain
                CREATE TABLE `Parent` (`Id` INT NOT NULL, PRIMARY KEY (`Id`));
                CREATE TABLE `Child` (`Id` INT NOT NULL, `ParentId` INT NOT NULL, PRIMARY KEY (`Id`),
                    CONSTRAINT `FK_Child_Parent` FOREIGN KEY (`ParentId`) REFERENCES `Parent` (`Id`));
                CREATE TABLE `GrandChild` (`Id` INT NOT NULL, `ChildId` INT NOT NULL, PRIMARY KEY (`Id`),
                    CONSTRAINT `FK_GrandChild_Child` FOREIGN KEY (`ChildId`) REFERENCES `Child` (`Id`));

                -- A cycle of 2 tables
                CREATE TABLE `CycleA` (`Id` INT NOT NULL, `BId` INT NULL, PRIMARY KEY (`Id`));
                CREATE TABLE `CycleB` (`Id` INT NOT NULL, `AId` INT NULL, PRIMARY KEY (`Id`),
                    CONSTRAINT `FK_CycleB_CycleA` FOREIGN KEY (`AId`) REFERENCES `CycleA` (`Id`));
                ALTER TABLE `CycleA` ADD CONSTRAINT `FK_CycleA_CycleB` FOREIGN KEY (`BId`) REFERENCES `CycleB` (`Id`);

                -- A composite foreign key and a second foreign key (with a cascading rule) in the same table
                CREATE TABLE `Shipment` (
                    `Id` INT NOT NULL,
                    `OrderId` INT NOT NULL,
                    `LineNumber` INT NOT NULL,
                    `CountryId` INT NOT NULL,
                    PRIMARY KEY (`Id`),
                    CONSTRAINT `FK_Shipment_OrderLine` FOREIGN KEY (`OrderId`, `LineNumber`) REFERENCES `OrderLine` (`OrderId`, `LineNumber`),
                    CONSTRAINT `FK_Shipment_Country` FOREIGN KEY (`CountryId`) REFERENCES `Country` (`Id`) ON DELETE CASCADE
                );

                -- A self-referencing foreign key
                CREATE TABLE `Employee` (`Id` INT NOT NULL, `ManagerId` INT NULL, PRIMARY KEY (`Id`),
                    CONSTRAINT `FK_Employee_Manager` FOREIGN KEY (`ManagerId`) REFERENCES `Employee` (`Id`));

                -- A foreign key with the RESTRICT rules
                CREATE TABLE `Preference` (
                    `Id` INT NOT NULL,
                    `CountryId` INT NOT NULL DEFAULT 1,
                    PRIMARY KEY (`Id`),
                    CONSTRAINT `FK_Preference_Country` FOREIGN KEY (`CountryId`) REFERENCES `Country` (`Id`) ON DELETE RESTRICT ON UPDATE RESTRICT
                );

                -- A table with 2 parents that share the same parent: A <- B, A <- C, B <- D, C <- D
                CREATE TABLE `DiamondA` (`Id` INT NOT NULL, PRIMARY KEY (`Id`));
                CREATE TABLE `DiamondB` (`Id` INT NOT NULL, `AId` INT NOT NULL, PRIMARY KEY (`Id`),
                    CONSTRAINT `FK_DiamondB_DiamondA` FOREIGN KEY (`AId`) REFERENCES `DiamondA` (`Id`));
                CREATE TABLE `DiamondC` (`Id` INT NOT NULL, `AId` INT NOT NULL, PRIMARY KEY (`Id`),
                    CONSTRAINT `FK_DiamondC_DiamondA` FOREIGN KEY (`AId`) REFERENCES `DiamondA` (`Id`));
                CREATE TABLE `DiamondD` (`Id` INT NOT NULL, `BId` INT NOT NULL, `CId` INT NOT NULL, PRIMARY KEY (`Id`),
                    CONSTRAINT `FK_DiamondD_DiamondB` FOREIGN KEY (`BId`) REFERENCES `DiamondB` (`Id`),
                    CONSTRAINT `FK_DiamondD_DiamondC` FOREIGN KEY (`CId`) REFERENCES `DiamondC` (`Id`));

                -- A table with 2 foreign keys to the same parent table
                CREATE TABLE `Account` (`Id` INT NOT NULL, PRIMARY KEY (`Id`));
                CREATE TABLE `Transfer` (`Id` INT NOT NULL, `FromAccountId` INT NOT NULL, `ToAccountId` INT NOT NULL, PRIMARY KEY (`Id`),
                    CONSTRAINT `FK_Transfer_FromAccount` FOREIGN KEY (`FromAccountId`) REFERENCES `Account` (`Id`),
                    CONSTRAINT `FK_Transfer_ToAccount` FOREIGN KEY (`ToAccountId`) REFERENCES `Account` (`Id`));

                -- A cycle of 3 tables: X -> Z -> Y -> X
                CREATE TABLE `RingX` (`Id` INT NOT NULL, `ZId` INT NULL, PRIMARY KEY (`Id`));
                CREATE TABLE `RingY` (`Id` INT NOT NULL, `XId` INT NULL, PRIMARY KEY (`Id`),
                    CONSTRAINT `FK_RingY_RingX` FOREIGN KEY (`XId`) REFERENCES `RingX` (`Id`));
                CREATE TABLE `RingZ` (`Id` INT NOT NULL, `YId` INT NULL, PRIMARY KEY (`Id`),
                    CONSTRAINT `FK_RingZ_RingY` FOREIGN KEY (`YId`) REFERENCES `RingY` (`Id`));
                ALTER TABLE `RingX` ADD CONSTRAINT `FK_RingX_RingZ` FOREIGN KEY (`ZId`) REFERENCES `RingZ` (`Id`);

                -- A cycle (A <-> B) with a table that the cycle depends on (Root) and a table that depends on the cycle (Leaf)
                CREATE TABLE `LoopRoot` (`Id` INT NOT NULL, PRIMARY KEY (`Id`));
                CREATE TABLE `LoopA` (`Id` INT NOT NULL, `RootId` INT NOT NULL, `BId` INT NULL, PRIMARY KEY (`Id`),
                    CONSTRAINT `FK_LoopA_LoopRoot` FOREIGN KEY (`RootId`) REFERENCES `LoopRoot` (`Id`));
                CREATE TABLE `LoopB` (`Id` INT NOT NULL, `AId` INT NULL, PRIMARY KEY (`Id`),
                    CONSTRAINT `FK_LoopB_LoopA` FOREIGN KEY (`AId`) REFERENCES `LoopA` (`Id`));
                ALTER TABLE `LoopA` ADD CONSTRAINT `FK_LoopA_LoopB` FOREIGN KEY (`BId`) REFERENCES `LoopB` (`Id`);
                CREATE TABLE `LoopLeaf` (`Id` INT NOT NULL, `BId` INT NOT NULL, PRIMARY KEY (`Id`),
                    CONSTRAINT `FK_LoopLeaf_LoopB` FOREIGN KEY (`BId`) REFERENCES `LoopB` (`Id`));

                -- A table with many children
                CREATE TABLE `FanRoot` (`Id` INT NOT NULL, PRIMARY KEY (`Id`));
                CREATE TABLE `FanChild1` (`Id` INT NOT NULL, `RootId` INT NOT NULL, PRIMARY KEY (`Id`), CONSTRAINT `FK_FanChild1_FanRoot` FOREIGN KEY (`RootId`) REFERENCES `FanRoot` (`Id`));
                CREATE TABLE `FanChild2` (`Id` INT NOT NULL, `RootId` INT NOT NULL, PRIMARY KEY (`Id`), CONSTRAINT `FK_FanChild2_FanRoot` FOREIGN KEY (`RootId`) REFERENCES `FanRoot` (`Id`));
                CREATE TABLE `FanChild3` (`Id` INT NOT NULL, `RootId` INT NOT NULL, PRIMARY KEY (`Id`), CONSTRAINT `FK_FanChild3_FanRoot` FOREIGN KEY (`RootId`) REFERENCES `FanRoot` (`Id`));
                CREATE TABLE `FanChild4` (`Id` INT NOT NULL, `RootId` INT NOT NULL, PRIMARY KEY (`Id`), CONSTRAINT `FK_FanChild4_FanRoot` FOREIGN KEY (`RootId`) REFERENCES `FanRoot` (`Id`));");

        private static void CreateIndexTables(IDbConnection connection) =>
            connection.ExecuteNonQuery(@"
                -- 2 unique indexes (a unique index is a unique constraint in MySQL), a multi-column index with a descending key and an index of one column
                CREATE TABLE `Product` (
                    `Id` INT NOT NULL,
                    `Code` VARCHAR(20) NOT NULL,
                    `Sku` VARCHAR(30) NOT NULL,
                    `Name` VARCHAR(100) NOT NULL,
                    `Category` INT NOT NULL,
                    `Price` DECIMAL(18, 2) NOT NULL,
                    `IsActive` TINYINT(1) NOT NULL DEFAULT 1,
                    PRIMARY KEY (`Id`)
                );
                CREATE UNIQUE INDEX `CIX_Product_Code` ON `Product` (`Code`);
                CREATE UNIQUE INDEX `UX_Product_Sku` ON `Product` (`Sku`);
                CREATE INDEX `IX_Product_Category_Price` ON `Product` (`Category` ASC, `Price` DESC);
                CREATE INDEX `IX_Product_Active_Name` ON `Product` (`Name`);

                -- The types
                CREATE TABLE `AllTypes` (
                    `CTinyInt` TINYINT NOT NULL,
                    `CSmallInt` SMALLINT NOT NULL,
                    `CMediumInt` MEDIUMINT NULL,
                    `CInt` INT NOT NULL,
                    `CBigInt` BIGINT NULL,
                    `CUnsignedInt` INT UNSIGNED NULL,
                    `CDecimal` DECIMAL(10, 3) NULL,
                    `CFloat` FLOAT NULL,
                    `CDouble` DOUBLE NULL,
                    `CBit` BIT(8) NULL,
                    `CBoolean` TINYINT(1) NULL,
                    `CChar` CHAR(5) NULL,
                    `CVarChar` VARCHAR(20) NULL,
                    `CTinyText` TINYTEXT NULL,
                    `CText` TEXT NULL,
                    `CMediumText` MEDIUMTEXT NULL,
                    `CLongText` LONGTEXT NULL,
                    `CBinary` BINARY(4) NULL,
                    `CVarBinary` VARBINARY(16) NULL,
                    `CBlob` BLOB NULL,
                    `CLongBlob` LONGBLOB NULL,
                    `CDate` DATE NULL,
                    `CTime` TIME(2) NULL,
                    `CDateTime` DATETIME(4) NULL,
                    `CDateTimeFree` DATETIME NULL,
                    `CTimestamp` TIMESTAMP(3) NULL,
                    `CYear` YEAR NULL,
                    `CEnum` ENUM('small', 'medium', 'large') NULL,
                    `CSet` SET('a', 'b', 'c') NULL,
                    `CJson` JSON NULL
                );");

        private static void CreateOddNameTables(IDbConnection connection) =>
            connection.ExecuteNonQuery(@"
                -- Tables whose names contain a dot, a space and a backtick
                CREATE TABLE `Odd.Name` (
                    `Id` INT NOT NULL,
                    `Value` VARCHAR(50) NULL,
                    PRIMARY KEY (`Id`)
                );
                CREATE INDEX `IX_OddName_Value` ON `Odd.Name` (`Value`);
                CREATE TABLE `Odd.Child` (
                    `Id` INT NOT NULL,
                    `ParentId` INT NOT NULL,
                    PRIMARY KEY (`Id`),
                    CONSTRAINT `FK_OddChild_OddName` FOREIGN KEY (`ParentId`) REFERENCES `Odd.Name` (`Id`)
                );
                CREATE TABLE `Order Details` (
                    `Id` INT NOT NULL,
                    `Unit Price` DECIMAL(10, 2) NOT NULL,
                    PRIMARY KEY (`Id`)
                );
                CREATE TABLE `Weird``Name` (
                    `Id` INT NOT NULL,
                    PRIMARY KEY (`Id`)
                );");

        #endregion
    }
}
