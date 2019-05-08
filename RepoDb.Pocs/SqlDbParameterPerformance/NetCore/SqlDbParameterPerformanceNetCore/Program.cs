using RepoDb;
using SqlDbParameterPerformanceNetCore.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace SqlDbParameterPerformanceNetCore
{
    class Program
    {
        private static readonly string m_connectionString = "Server=(local);Database=RepoDb;Integrated Security=True;";

        static void Main(string[] args)
        {
            TypeMapper.Map(typeof(DateTime), DbType.DateTime2);
            var iterations = 10;
            var rows = 1000;
            Excercise(iterations, rows);
            InsertAllViaRepoDb(iterations, rows);
            InsertViaRepoDb(iterations, rows);
            InsertViaClearAndCreate(iterations, rows);
            InsertViaParameterAssignmentByName(iterations, rows);
            InsertViaParameterAssignmentByIndex(iterations, rows);
            Console.ReadLine();
        }

        private static void Excercise(int iterations, int rows)
        {
            Console.WriteLine("Exercising, please wait...");
            InsertAllViaRepoDb(iterations, rows, false);
            InsertViaRepoDb(iterations, rows, false);
            InsertViaClearAndCreate(iterations, rows, false);
            InsertViaParameterAssignmentByName(iterations, rows, false);
            InsertViaParameterAssignmentByIndex(iterations, rows, false);
            Console.WriteLine("Exercise completed.");
            Task.Delay(2000);
        }

        private static void InsertAllViaRepoDb(int iterations, int rows, bool log = true)
        {
            using (var connection = new SqlConnection(m_connectionString))
            {
                var identityTables = CreateIdentityTables(rows);
                connection.Truncate<IdentityTable>();
                var stopwatch = new Stopwatch();
                var milliseconds = new List<double>();
                for (var i = 0; i < iterations; i++)
                {
                    stopwatch.Start();
                    connection.InsertAll(identityTables, 1);
                    stopwatch.Stop();
                    milliseconds.Add(stopwatch.Elapsed.TotalMilliseconds);
                    stopwatch.Reset();
                }
                if (log)
                {
                    Console.WriteLine($"RepoDb InsertAll: {milliseconds.Average() / 1000} millisecond(s) for {iterations} iteration(s) with {rows} row(s) each.");
                }
            }
        }

        private static void InsertViaRepoDb(int iterations, int rows, bool log = true)
        {
            using (var connection = new SqlConnection(m_connectionString))
            {
                var identityTables = CreateIdentityTables(rows);
                connection.Truncate<IdentityTable>();
                var stopwatch = new Stopwatch();
                var milliseconds = new List<double>();
                for (var i = 0; i < iterations; i++)
                {
                    stopwatch.Start();
                    identityTables.AsList().ForEach(item => connection.Insert(item));
                    stopwatch.Stop();
                    milliseconds.Add(stopwatch.Elapsed.TotalMilliseconds);
                    stopwatch.Reset();
                }
                if (log)
                {
                    Console.WriteLine($"RepoDb Insert: {milliseconds.Average() / 1000} millisecond(s) for {iterations} iteration(s) with {rows} row(s) each.");
                }
            }
        }

        private static void InsertViaClearAndCreate(int iterations, int rows, bool log = true)
        {
            using (var connection = new SqlConnection(m_connectionString).EnsureOpen())
            {
                var identityTables = CreateIdentityTables(rows);
                connection.Truncate<IdentityTable>();
                var stopwatch = new Stopwatch();
                var milliseconds = new List<double>();

                for (var i = 0; i < iterations; i++)
                {
                    using (var command = connection.CreateCommand(@"INSERT INTO [sc].[IdentityTable] (
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
                    SET @Result = SCOPE_IDENTITY();"))
                    {
                        // Execution
                        stopwatch.Start();
                        foreach (var item in identityTables)
                        {
                            // Clear the parameters
                            command.Parameters.Clear();

                            // Parameters
                            CreateParameterWithValue(command, "RowGuid", item.RowGuid, DbType.Guid, 16, 0, 0);
                            CreateParameterWithValue(command, "ColumnBit", item.ColumnBit, DbType.Boolean, 1, 1, 0);
                            CreateParameterWithValue(command, "ColumnDateTime", item.ColumnDateTime, DbType.DateTime, 8, 23, 3);
                            CreateParameterWithValue(command, "ColumnDateTime2", item.ColumnDateTime2, DbType.DateTime2, 8, 27, 7);
                            CreateParameterWithValue(command, "ColumnDecimal", item.ColumnDecimal, DbType.Decimal, 9, 18, 2);
                            CreateParameterWithValue(command, "ColumnFloat", item.ColumnFloat, DbType.Single, 8, 53, 0);
                            CreateParameterWithValue(command, "ColumnInt", item.ColumnInt, DbType.Int32, 4, 10, 0);
                            CreateParameterWithValue(command, "ColumnNVarChar", item.ColumnNVarChar, DbType.String, -1, 0, 0);
                            CreateParameterWithValue(command, "Result", null, DbType.Int64, 8, 19, 0, ParameterDirection.Output);

                            // Prepare
                            command.Prepare();

                            // Execute
                            command.ExecuteNonQuery();

                            // Set the result back
                            item.Id = (long)((IDbDataParameter)command.Parameters["Result"]).Value;
                        }
                        stopwatch.Stop();
                        milliseconds.Add(stopwatch.Elapsed.TotalMilliseconds);
                        stopwatch.Reset();

                        // Dispose
                        command.Dispose();
                    }
                }

                // Log
                if (log)
                {
                    Console.WriteLine($"HandCoded ClearAndCreate: {milliseconds.Average() / 1000} millisecond(s) for {iterations} iteration(s) with {rows} row(s) each.");
                }
            }
        }

        private static void InsertViaParameterAssignmentByName(int iterations, int rows, bool log = true)
        {
            using (var connection = new SqlConnection(m_connectionString).EnsureOpen())
            {
                var identityTables = CreateIdentityTables(rows);
                connection.Truncate<IdentityTable>();
                var stopwatch = new Stopwatch();
                var milliseconds = new List<double>();

                for (var i = 0; i < iterations; i++)
                {
                    using (var command = connection.CreateCommand(@"INSERT INTO [sc].[IdentityTable] (
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
                    SET @Result = SCOPE_IDENTITY();"))
                    {
                        // Add the parameters
                        CreateParameter(command, "RowGuid", DbType.Guid, 16, 0, 0);
                        CreateParameter(command, "ColumnBit", DbType.Boolean, 1, 1, 0);
                        CreateParameter(command, "ColumnDateTime", DbType.DateTime, 8, 23, 3);
                        CreateParameter(command, "ColumnDateTime2", DbType.DateTime2, 8, 27, 7);
                        CreateParameter(command, "ColumnDecimal", DbType.Decimal, 9, 18, 2);
                        CreateParameter(command, "ColumnFloat", DbType.Single, 8, 53, 0);
                        CreateParameter(command, "ColumnInt", DbType.Int32, 4, 10, 0);
                        CreateParameter(command, "ColumnNVarChar", DbType.String, -1, 0, 0);
                        CreateParameter(command, "Result", DbType.Int64, 8, 19, 0, ParameterDirection.Output);

                        // Prepare
                        command.Prepare();

                        // Execution
                        stopwatch.Start();
                        foreach (var item in identityTables)
                        {
                            // Parameters
                            ((IDbDataParameter)command.Parameters["RowGuid"]).Value = item.RowGuid;
                            ((IDbDataParameter)command.Parameters["ColumnBit"]).Value = item.ColumnBit;
                            ((IDbDataParameter)command.Parameters["ColumnDateTime"]).Value = item.ColumnDateTime;
                            ((IDbDataParameter)command.Parameters["ColumnDateTime2"]).Value = item.ColumnDateTime2;
                            ((IDbDataParameter)command.Parameters["ColumnDecimal"]).Value = item.ColumnDecimal;
                            ((IDbDataParameter)command.Parameters["ColumnFloat"]).Value = item.ColumnFloat;
                            ((IDbDataParameter)command.Parameters["ColumnInt"]).Value = item.ColumnInt;
                            ((IDbDataParameter)command.Parameters["ColumnNVarChar"]).Value = item.ColumnNVarChar;

                            // Execute
                            command.ExecuteNonQuery();

                            // Set the result back
                            item.Id = (long)((IDbDataParameter)command.Parameters["Result"]).Value;
                        }
                        stopwatch.Stop();
                        milliseconds.Add(stopwatch.Elapsed.TotalMilliseconds);
                        stopwatch.Reset();
                    }
                }

                // Log
                if (log)
                {
                    Console.WriteLine($"HandCoded ParameterAssignment (By Name): {milliseconds.Average() / 1000} millisecond(s) for {iterations} iteration(s) with {rows} row(s) each.");
                }
            }
        }

        private static void InsertViaParameterAssignmentByIndex(int iterations, int rows, bool log = true)
        {
            using (var connection = new SqlConnection(m_connectionString).EnsureOpen())
            {
                var identityTables = CreateIdentityTables(rows);
                connection.Truncate<IdentityTable>();
                var stopwatch = new Stopwatch();
                var milliseconds = new List<double>();

                for (var i = 0; i < iterations; i++)
                {
                    using (var command = connection.CreateCommand(@"INSERT INTO [sc].[IdentityTable] (
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
                    SET @Result = SCOPE_IDENTITY();"))
                    {
                        // Add the parameters
                        CreateParameter(command, "RowGuid", DbType.Guid, 16, 0, 0);
                        CreateParameter(command, "ColumnBit", DbType.Boolean, 1, 1, 0);
                        CreateParameter(command, "ColumnDateTime", DbType.DateTime, 8, 23, 3);
                        CreateParameter(command, "ColumnDateTime2", DbType.DateTime2, 8, 27, 7);
                        CreateParameter(command, "ColumnDecimal", DbType.Decimal, 9, 18, 2);
                        CreateParameter(command, "ColumnFloat", DbType.Single, 8, 53, 0);
                        CreateParameter(command, "ColumnInt", DbType.Int32, 4, 10, 0);
                        CreateParameter(command, "ColumnNVarChar", DbType.String, -1, 0, 0);
                        CreateParameter(command, "Result", DbType.Int64, 8, 19, 0, ParameterDirection.Output);

                        // Prepare
                        command.Prepare();

                        // Execution
                        stopwatch.Start();
                        foreach (var item in identityTables)
                        {
                            // Parameters
                            ((IDbDataParameter)command.Parameters[0]).Value = item.RowGuid;
                            ((IDbDataParameter)command.Parameters[1]).Value = item.ColumnBit;
                            ((IDbDataParameter)command.Parameters[2]).Value = item.ColumnDateTime;
                            ((IDbDataParameter)command.Parameters[3]).Value = item.ColumnDateTime2;
                            ((IDbDataParameter)command.Parameters[4]).Value = item.ColumnDecimal;
                            ((IDbDataParameter)command.Parameters[5]).Value = item.ColumnFloat;
                            ((IDbDataParameter)command.Parameters[6]).Value = item.ColumnInt;
                            ((IDbDataParameter)command.Parameters[7]).Value = item.ColumnNVarChar;

                            // Execute
                            command.ExecuteNonQuery();

                            // Set the result back
                            item.Id = (long)((IDbDataParameter)command.Parameters[8]).Value;
                        }
                        stopwatch.Stop();
                        milliseconds.Add(stopwatch.Elapsed.TotalMilliseconds);
                        stopwatch.Reset();
                    }
                }

                // Log
                if (log)
                {
                    Console.WriteLine($"HandCoded ParameterAssignment (By Index): {milliseconds.Average() / 1000} millisecond(s) for {iterations} iteration(s) with {rows} row(s) each.");
                }
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
                    ColumnDecimal = 100.00M, // Convert.ToDecimal(i),
                    ColumnFloat = Convert.ToSingle(i),
                    ColumnInt = i,
                    ColumnNVarChar = $"NVARCHAR{index}"
                };
            }
        }

        private static IDbDataParameter CreateParameter(IDbCommand command,
            string name,
            DbType dbType,
            int size,
            byte precision,
            byte scale,
            ParameterDirection direction = ParameterDirection.Input)
        {
            var parameter = command.CreateParameter();
            parameter.ParameterName = name;
            parameter.DbType = dbType;
            parameter.Size = size;
            parameter.Scale = scale;
            parameter.Direction = direction;
            command.Parameters.Add(parameter);
            return parameter;
        }

        private static IDbDataParameter CreateParameterWithValue(IDbCommand command,
            string name,
            object value,
            DbType dbType,
            int size,
            byte precision,
            byte scale,
            ParameterDirection direction = ParameterDirection.Input)
        {
            var parameter = command.CreateParameter();
            parameter.ParameterName = name;
            parameter.DbType = dbType;
            parameter.Size = size;
            parameter.Value = value;
            parameter.Precision = precision;
            parameter.Scale = scale;
            parameter.Direction = direction;
            command.Parameters.Add(parameter);
            return parameter;
        }
    }
}
