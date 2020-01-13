using Microsoft.VisualStudio.TestTools.UnitTesting;
using Npgsql;
using RepoDb.Extensions;
using RepoDb.PostgreSql.IntegrationTests.Models;
using RepoDb.PostgreSql.IntegrationTests.Setup;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RepoDb.PostgreSql.IntegrationTests.Operations
{
    [TestClass]
    public class _Test
    {
        [TestInitialize]
        public void Initialize()
        {
            Database.Initialize();
            //Cleanup();
        }

        [TestCleanup]
        public void Cleanup()
        {
            //Database.Cleanup();
        }

        [TestMethod]
        public void TestConnection()
        {
            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                var result = connection.QueryAll<CompleteTable>();
            }
        }

        [TestMethod]
        public void TestExtractProperties()
        {
            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                var entities = (string)null;
                using (var reader = connection.ExecuteReader("SELECT * FROM \"CompleteTable\" WHERE 1 = 0;"))
                {
                    for (var i = 0; i < reader.FieldCount; i++)
                    {
                        var name = reader.GetName(i);
                        var type = reader.GetFieldType(i);
                        if (type.IsValueType)
                        {
                            entities = string.Concat(entities, $"public System.Nullable<{type.FullName}> {name} {{ get; set; }}\n");
                        }
                        else
                        {
                            entities = string.Concat(entities, $"public {type.FullName} {name} {{ get; set; }}\n");
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void TestExtractForPostgreSqlTypeNameToClientTypeResolver()
        {
            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                var dbFields = DbFieldCache.Get(connection, "CompleteTable", null).AsList();
                var dictionary = new Dictionary<Type, List<string>>();

                using (var reader = connection.ExecuteReader(GetSelectText()))
                {
                    for (var i = 0; i < reader.FieldCount; i++)
                    {
                        var name = reader.GetName(i);
                        var type = reader.GetFieldType(i);
                        var dbField = dbFields
                            .FirstOrDefault(e =>
                                string.Equals(e.Name, name, StringComparison.CurrentCultureIgnoreCase));

                        if (dbField == null)
                        {
                            continue;
                        }

                        var databaseType = dbField.DatabaseType.ToLower();
                        var list = (List<string>)null;

                        if (dictionary.ContainsKey(type))
                        {
                            list = dictionary[type];
                        }
                        else
                        {
                            list = new List<string>();
                            dictionary[type] = list;
                        }

                        if (!list.Contains(databaseType))
                        {
                            list.Add(databaseType);
                        }
                    }
                }

                var entries = (string)null;
                foreach (var kvp in dictionary)
                {
                    var databaseTypes = kvp.Value;
                    foreach (var databaseType in databaseTypes)
                    {
                        entries = string.Concat(entries, $"case \"{databaseType}\":\n");
                    }
                    entries = string.Concat(entries, $"\treturn typeof({kvp.Key.FullName});\n");
                }
            }
        }

        private string GetSelectText()
        {
            return "SELECT \"Id\", \"ColumnChar\", \"ColumnCharAsArray\", \"ColumnBigInt\", \"ColumnBigIntAsArray\", \"ColumnBigSerial\", \"ColumnBit\", \"ColumnBitVarying\", \"ColumnBitVaryingAsArray\", \"ColumnBitAsArray\", \"ColumnBoolean\", \"ColumnBooleanAsArray\", \"ColumnBox\", \"ColumnBoxAsArray\", \"ColumnByteA\", \"ColumnByteAAsArray\", \"ColumnCharacter\", \"ColumnCharacterVarying\", \"ColumnCharacterVaryingAsArray\", \"ColumnCid\", \"ColumnCidAsArray\", \"ColumnCircle\", \"ColumnCircleAsArray\", \"ColumnDate\", \"ColumnDateAsArray\", \"ColumnDateRangeAsArray\", \"ColumnDoublePrecision\", \"ColumnDoublePrecisionAsArray\", \"ColumnInet\", \"ColumnInetAsArray\", \"ColumnInt2Vector\", \"ColumnInt2VectorAsArray\", \"ColumnInt4RangeAsArray\", \"ColumnInt8RangeAsArray\", \"ColumnInteger\", \"ColumnIntegerAsArray\", \"ColumnInterval\", \"ColumnIntervalAsArray\", \"ColumnJson\", \"ColumnJsonAsArray\", \"ColumnJsonB\", \"ColumnJsonBAsArray\", \"ColumnJsonPath\", \"ColumnJsonPathAsArray\", \"ColumnLine\", \"ColumnLineAsArray\", \"ColumnLSeg\", \"ColumnLSegAsArray\", \"ColumnMacAddr\", \"ColumnMacAddrAsArray\", \"ColumnMacAddr8\", \"ColumnMacAddr8AsArray\", \"ColumMoney\", \"ColumnMoneyAsArray\", \"ColumName\", \"ColumnNameAsArray\", \"ColumnNumeric\", \"ColumnNumericAsArray\", \"ColumnNumRangeAsArray\", \"ColumnOId\", \"ColumnOIdAsArray\", \"ColumnOIdVector\", \"ColumnOIdVectorAsArray\", \"ColumnPath\", \"ColumnPathAsArray\", \"ColumnPgDependencies\", \"ColumnPgLsn\", \"ColumnPgLsnAsArray\", \"ColumnPgMcvList\", \"ColumnPgNDistinct\", \"ColumnPgNodeTree\", \"ColumnPoint\", \"ColumnPointAsArray\", \"ColumnPolygon\", \"ColumnPolygonAsArray\", \"ColumnReal\", \"ColumnRealAsArray\", \"ColumnRefCursor\", \"ColumnRefCursorAsArray\", \"ColumnRegClass\", \"ColumnRegClassAsArray\", \"ColumnRegConfig\", \"ColumnRegConfigAsArray\", \"ColumnRegDictionary\", \"ColumnRegDictionaryAsArray\", \"ColumnRegNamespace\", \"ColumnRegNamespaceAsArray\", \"ColumnRegOper\", \"ColumnRegOperAsArray\", \"ColumnRegOperator\", \"ColumnRegOperationAsArray\", \"ColumnRegProc\", \"ColumnRegProcAsArray\", \"ColumnRegProcedure\", \"ColumnRegProcedureAsArray\", \"ColumnRegRole\", \"ColumnRegRoleAsArray\", \"ColumnRegType\", \"ColumnRegTypeAsArray\", \"ColumnSerial\", \"ColumnSmallInt\", \"ColumnSmallIntAsArray\", \"ColumnSmallSerial\", \"ColumnText\", \"ColumnTextAsArray\", \"ColumnTId\", \"ColumnTidAsArray\", \"ColumnTimeWithTimeZoneAsArray\", \"ColumnTimeWithTimeZone\", \"ColumnTimeWithoutTimeZone\", \"ColumnTimeWithoutTimeZoneAsArray\", \"ColumnTimestampWithTimeZone\", \"ColumnTimestampWithTimeZoneAsArray\", \"ColumnTimestampWithoutTimeZone\", \"ColumnTimestampWithoutTimeZoneAsArray\", \"ColumnTSQuery\", \"ColumnTSQueryAsArray\", \"ColumnTSRangeAsArray\", \"ColumnTSTZRangeAsArray\", \"ColumnTSVector\", \"ColumnTSVectorAsArray\", \"ColumnTXIDSnapshot\", \"ColumnTXIDSnapshotAsArray\", \"ColumnUUID\", \"ColumnUUIDAsArray\", \"ColumnXID\", \"ColumnXIDAsArray\", \"ColumnXML\", \"ColumnXMLAsArray\" FROM \"CompleteTable\" ;";
        }
    }
}
