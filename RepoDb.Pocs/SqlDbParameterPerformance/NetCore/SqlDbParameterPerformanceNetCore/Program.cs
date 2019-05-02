using RepoDb;
using SqlDbParameterPerformanceNetCore.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;

namespace SqlDbParameterPerformanceNetCore
{
    class Program
    {
        private static readonly string m_connectionString = "Server=(local);Database=RepoDb;Integrated Security=True;";

        static void Main(string[] args)
        {
            TypeMapper.Map(typeof(DateTime), DbType.DateTime2);
            var rows = 500;
            Excercise();
            InsertAllViaRepoDb(rows);
            //InsertViaRepoDb(rows);
            InsertViaClearAndCreate(rows);
            InsertViaParameterAssignmentByName(rows);
            InsertViaParameterAssignmentByIndex(rows);
            Console.ReadLine();
        }

        private static void Excercise()
        {
            using (var connection = new SqlConnection(m_connectionString).EnsureOpen())
            {
                var identityTables = CreateIdentityTables(10);
                connection.InsertAll(identityTables);
                connection.Truncate<IdentityTable>();
            }
        }

        private static void InsertViaRepoDb(int rows)
        {
            using (var connection = new SqlConnection(m_connectionString).EnsureOpen())
            {
                var identityTables = CreateIdentityTables(rows);
                var stopwatch = new Stopwatch();
                connection.Truncate<IdentityTable>();
                stopwatch.Start();
                foreach (var item in identityTables)
                {
                    connection.Insert(item);
                }
                stopwatch.Stop();
                Console.WriteLine($"RepoDb Insert: {stopwatch.Elapsed.TotalSeconds} second(s) for {rows} row(s).");
            }
        }

        private static void InsertAllViaRepoDb(int rows)
        {
            using (var connection = new SqlConnection(m_connectionString))
            {
                var identityTables = CreateIdentityTables(rows);
                var stopwatch = new Stopwatch();
                connection.Truncate<IdentityTable>();
                stopwatch.Start();
                connection.InsertAll(identityTables);
                stopwatch.Stop();
                Console.WriteLine($"RepoDb InsertAll: {stopwatch.Elapsed.TotalSeconds} second(s) for {rows} row(s).");
            }
        }

        private static void InsertViaClearAndCreate(int rows)
        {
            using (var connection = new SqlConnection(m_connectionString).EnsureOpen())
            {
                // Variables
                var identityTables = CreateIdentityTables(rows);
                var stopwatch = new Stopwatch();
                var command = connection.CreateCommand(@"INSERT INTO [dbo].[IdentityTable] (
                        RowGuid
                        , ColumnBit
                        , ColumnDateTime
                        , ColumnDateTime2
                        , ColumnDecimal
                        , ColumnFloat
                        , ColumnInt
                        , ColumnNVarChar
                    )
                    VALUES
                    (
                        @RowGuid
                        , @ColumnBit
                        , @ColumnDateTime
                        , @ColumnDateTime2
                        , @ColumnDecimal
                        , @ColumnFloat
                        , @ColumnInt
                        , @ColumnNVarChar
                    );
                    SELECT SCOPE_IDENTITY() AS RESULT;");

                // Execution
                connection.Truncate<IdentityTable>();
                stopwatch.Start();
                foreach (var item in identityTables)
                {
                    // Clear the parameters
                    command.Parameters.Clear();

                    // Parameters
                    CreateParameterWithValue(command, "RowGuid", item.RowGuid);
                    CreateParameterWithValue(command, "ColumnBit", item.ColumnBit);
                    CreateParameterWithValue(command, "ColumnDateTime", item.ColumnDateTime);
                    CreateParameterWithValue(command, "ColumnDateTime2", item.ColumnDateTime2);
                    CreateParameterWithValue(command, "ColumnDecimal", item.ColumnDecimal);
                    CreateParameterWithValue(command, "ColumnFloat", item.ColumnFloat);
                    CreateParameterWithValue(command, "ColumnInt", item.ColumnInt);
                    CreateParameterWithValue(command, "ColumnNVarChar", item.ColumnNVarChar);

                    // Execute
                    var result = command.ExecuteScalar();
                }
                stopwatch.Stop();
                Console.WriteLine($"HandCoded ClearAndCreate: {stopwatch.Elapsed.TotalSeconds} second(s) for {rows} row(s).");
            }
        }

        private static void InsertViaParameterAssignmentByName(int rows)
        {
            using (var connection = new SqlConnection(m_connectionString).EnsureOpen())
            {
                // Variables
                var identityTables = CreateIdentityTables(rows);
                var stopwatch = new Stopwatch();
                var command = connection.CreateCommand(@"INSERT INTO [dbo].[IdentityTable] (
                        RowGuid
                        , ColumnBit
                        , ColumnDateTime
                        , ColumnDateTime2
                        , ColumnDecimal
                        , ColumnFloat
                        , ColumnInt
                        , ColumnNVarChar
                    )
                    VALUES
                    (
                        @RowGuid
                        , @ColumnBit
                        , @ColumnDateTime
                        , @ColumnDateTime2
                        , @ColumnDecimal
                        , @ColumnFloat
                        , @ColumnInt
                        , @ColumnNVarChar
                    );
                    SELECT SCOPE_IDENTITY() AS RESULT;");

                // Add the parameters
                CreateParameter(command, "RowGuid");
                CreateParameter(command, "ColumnBit");
                CreateParameter(command, "ColumnDateTime");
                CreateParameter(command, "ColumnDateTime2");
                CreateParameter(command, "ColumnDecimal");
                CreateParameter(command, "ColumnFloat");
                CreateParameter(command, "ColumnInt");
                CreateParameter(command, "ColumnNVarChar");

                // Execution
                connection.Truncate<IdentityTable>();
                stopwatch.Start();
                foreach (var item in identityTables)
                {
                    // Parameters
                    ((IDbDataParameter)command.Parameters["RowGuid"]).Value = item.ColumnBit;
                    ((IDbDataParameter)command.Parameters["ColumnBit"]).Value = item.ColumnBit;
                    ((IDbDataParameter)command.Parameters["ColumnDateTime"]).Value = item.ColumnDateTime;
                    ((IDbDataParameter)command.Parameters["ColumnDateTime2"]).Value = item.ColumnDateTime2;
                    ((IDbDataParameter)command.Parameters["ColumnDecimal"]).Value = item.ColumnDecimal;
                    ((IDbDataParameter)command.Parameters["ColumnFloat"]).Value = item.ColumnFloat;
                    ((IDbDataParameter)command.Parameters["ColumnInt"]).Value = item.ColumnInt;
                    ((IDbDataParameter)command.Parameters["ColumnNVarChar"]).Value = item.ColumnNVarChar;

                    // Execute
                    var result = command.ExecuteScalar();
                }
                stopwatch.Stop();
                Console.WriteLine($"HandCoded ParameterAssignment (By Name): {stopwatch.Elapsed.TotalSeconds} second(s) for {rows} row(s).");
            }
        }

        private static void InsertViaParameterAssignmentByIndex(int rows)
        {
            using (var connection = new SqlConnection(m_connectionString).EnsureOpen())
            {
                // Variables
                var identityTables = CreateIdentityTables(rows);
                var stopwatch = new Stopwatch();
                var command = connection.CreateCommand(@"INSERT INTO [dbo].[IdentityTable] (
                        RowGuid
                        , ColumnBit
                        , ColumnDateTime
                        , ColumnDateTime2
                        , ColumnDecimal
                        , ColumnFloat
                        , ColumnInt
                        , ColumnNVarChar
                    )
                    VALUES
                    (
                        @RowGuid
                        , @ColumnBit
                        , @ColumnDateTime
                        , @ColumnDateTime2
                        , @ColumnDecimal
                        , @ColumnFloat
                        , @ColumnInt
                        , @ColumnNVarChar
                    );
                    SELECT SCOPE_IDENTITY() AS RESULT;");

                // Add the parameters
                CreateParameter(command, "RowGuid");
                CreateParameter(command, "ColumnBit");
                CreateParameter(command, "ColumnDateTime");
                CreateParameter(command, "ColumnDateTime2");
                CreateParameter(command, "ColumnDecimal");
                CreateParameter(command, "ColumnFloat");
                CreateParameter(command, "ColumnInt");
                CreateParameter(command, "ColumnNVarChar");

                // Execution
                connection.Truncate<IdentityTable>();
                stopwatch.Start();
                foreach (var item in identityTables)
                {
                    // Parameters
                    ((IDbDataParameter)command.Parameters[0]).Value = item.ColumnBit;
                    ((IDbDataParameter)command.Parameters[1]).Value = item.ColumnBit;
                    ((IDbDataParameter)command.Parameters[2]).Value = item.ColumnDateTime;
                    ((IDbDataParameter)command.Parameters[3]).Value = item.ColumnDateTime2;
                    ((IDbDataParameter)command.Parameters[4]).Value = item.ColumnDecimal;
                    ((IDbDataParameter)command.Parameters[5]).Value = item.ColumnFloat;
                    ((IDbDataParameter)command.Parameters[6]).Value = item.ColumnInt;
                    ((IDbDataParameter)command.Parameters[7]).Value = item.ColumnNVarChar;

                    // Execute
                    var result = command.ExecuteScalar();
                }
                stopwatch.Stop();
                Console.WriteLine($"HandCoded ParameterAssignment (By Index): {stopwatch.Elapsed.TotalSeconds} second(s) for {rows} row(s).");
            }
        }


        private static IEnumerable<IdentityTable> CreateIdentityTables(int count)
        {
            for (var i = 0; i < count; i++)
            {
                var index = i + 1;
                yield return new IdentityTable
                {
                    RowGuid = Guid.NewGuid(),
                    ColumnBit = true,
                    ColumnDateTime = DateTime.UtcNow.Date,
                    ColumnDateTime2 = DateTime.UtcNow,
                    ColumnDecimal = index,
                    ColumnFloat = index,
                    ColumnInt = index,
                    ColumnNVarChar = $"NVARCHAR{index}"
                };
            }
        }


        private static IDbDataParameter CreateParameter(IDbCommand command, string name)
        {
            var parameter = command.CreateParameter();
            parameter.ParameterName = name;
            command.Parameters.Add(parameter);
            return parameter;
        }

        private static IDbDataParameter CreateParameterWithValue(IDbCommand command, string name, object value)
        {
            var parameter = command.CreateParameter();
            parameter.ParameterName = name;
            parameter.Value = value;
            command.Parameters.Add(parameter);
            return parameter;
        }
    }
}
