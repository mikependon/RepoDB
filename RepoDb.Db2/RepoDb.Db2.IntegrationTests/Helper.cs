#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.Db2.IntegrationTests.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace RepoDb.Db2.IntegrationTests
{
    public static class Helper
    {
        private static readonly Random m_random = new();

        public static IEnumerable<CompleteTable> CreateCompleteTables(int count)
        {
            for (var i = 0; i < count; i++)
            {
                yield return new CompleteTable
                {
                    SessionId = Guid.NewGuid(),
                    ColumnVarchar = $"Value-{m_random.Next(int.MaxValue)}",
                    ColumnNumber = Math.Round(Convert.ToDecimal(m_random.NextDouble() * 1000), 12),
                    ColumnDate = DateTime.UtcNow.Date,
                    ColumnTimestamp = DateTime.UtcNow,

                    ColumnVarchar2 = $"Value2-{m_random.Next(int.MaxValue)}",
                    // Generated at exactly the declared CHAR(10)/GRAPHIC(10) length, so Db2's
                    // blank-padding behavior for under-length CHAR/GRAPHIC values never kicks in -
                    // sidesteps needing a trim-aware comparison in AssertPropertiesEquality.
                    ColumnChar = GetFixedLengthString(10),
                    ColumnNChar = GetFixedLengthString(10),

                    ColumnInt = m_random.Next(int.MinValue, int.MaxValue),
                    ColumnBigInt = ((long)m_random.Next() << 32) | (uint)m_random.Next(),
                    ColumnSmallInt = (short)m_random.Next(short.MinValue, short.MaxValue),
                    ColumnTinyInt = (byte)m_random.Next(byte.MinValue, byte.MaxValue),

                    ColumnBinaryFloat = (float)Math.Round(m_random.NextDouble() * 1000, 4),
                    ColumnBinaryDouble = Math.Round(m_random.NextDouble() * 1000, 8),

                    ColumnRaw = Guid.NewGuid().ToByteArray()
                };
            }
        }

        public static IEnumerable<NonIdentityCompleteTable> CreateNonIdentityCompleteTables(int count)
        {
            for (var i = 0; i < count; i++)
            {
                yield return new NonIdentityCompleteTable
                {
                    Id = i + 1,
                    SessionId = Guid.NewGuid(),
                    ColumnVarchar = $"Value-{m_random.Next(int.MaxValue)}",
                    ColumnNumber = Math.Round(Convert.ToDecimal(m_random.NextDouble() * 1000), 12),
                    ColumnDate = DateTime.UtcNow.Date,
                    ColumnTimestamp = DateTime.UtcNow,

                    ColumnVarchar2 = $"Value2-{m_random.Next(int.MaxValue)}",
                    ColumnChar = GetFixedLengthString(10),
                    ColumnNChar = GetFixedLengthString(10),

                    ColumnInt = m_random.Next(int.MinValue, int.MaxValue),
                    ColumnBigInt = ((long)m_random.Next() << 32) | (uint)m_random.Next(),
                    ColumnSmallInt = (short)m_random.Next(short.MinValue, short.MaxValue),
                    ColumnTinyInt = (byte)m_random.Next(byte.MinValue, byte.MaxValue),

                    ColumnBinaryFloat = (float)Math.Round(m_random.NextDouble() * 1000, 4),
                    ColumnBinaryDouble = Math.Round(m_random.NextDouble() * 1000, 8),

                    ColumnRaw = Guid.NewGuid().ToByteArray()
                };
            }
        }

        private static string GetFixedLengthString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Range(0, length).Select(_ => chars[m_random.Next(chars.Length)]).ToArray());
        }

        public static void AssertPropertiesEquality<T>(T t1,
            T t2)
        {
            if (ReferenceEquals(t1, t2))
            {
                return;
            }
            if (t1 == null || t2 == null)
            {
                throw new InvalidOperationException("One of the objects is null.");
            }
            foreach (var property in typeof(T).GetProperties())
            {
                var value1 = property.GetValue(t1);
                var value2 = property.GetValue(t2);
                if (value1 is DateTime d1 && value2 is DateTime d2)
                {
                    // Db2 DATE/TIMESTAMP (without "WITH TIME ZONE") store no timezone info, so
                    // ODP.NET always reads them back as Kind=Unspecified. Normalize both sides to
                    // Unspecified before formatting, so a Utc-vs-Unspecified Kind mismatch doesn't
                    // surface as a false "O"-format ('Z' suffix) inequality.
                    // User of the library should handle their own type in their Db2 database.
                    //
                    // Db2's TIMESTAMP column ("ColumnTimestamp" in Database.cs, declared without an
                    // explicit precision) defaults to TIMESTAMP(6) - microsecond (6 fractional digit)
                    // resolution - one digit coarser than .NET DateTime's native 100-nanosecond (7
                    // fractional digit) resolution. A value generated via DateTime.UtcNow (which
                    // carries sub-microsecond ticks) therefore has its trailing decimal digit
                    // truncated (not rounded) on a round-trip through the database - e.g.
                    // ".7038701" comes back as ".7038700". Truncate both sides down to that same
                    // microsecond boundary (the nearest 10-tick/1-microsecond floor) before
                    // formatting, so this expected precision loss isn't mistaken for a bug.
                    var truncatedD1 = new DateTime(d1.Ticks - (d1.Ticks % 10), d1.Kind);
                    var truncatedD2 = new DateTime(d2.Ticks - (d2.Ticks % 10), d2.Kind);
                    (value1, value2) = (
                        DateTime.SpecifyKind(truncatedD1, DateTimeKind.Unspecified).ToString("O", CultureInfo.InvariantCulture),
                        DateTime.SpecifyKind(truncatedD2, DateTimeKind.Unspecified).ToString("O", CultureInfo.InvariantCulture));
                }
                else if (value1 is byte[] bytes1 && value2 is byte[] bytes2)
                {
                    // Arrays use reference equality under Equals()/object.Equals() unless overridden,
                    // so a plain Equals(value1, value2) below would always fail for two distinct byte[]
                    // instances even when their contents are identical (e.g. RAW(16) round-trips).
                    // Compare element-by-element instead.
                    Assert(bytes1.SequenceEqual(bytes2), property.Name, value1, value2);
                    continue;
                }
                Assert(Equals(value1, value2), property.Name, value1, value2);
            }
        }

        private static void Assert(bool condition,
            string propertyName,
            object value1,
            object value2)
        {
            if (!condition)
            {
                throw new InvalidOperationException($"The property '{propertyName}' values '{value1}' and '{value2}' are not equal.");
            }
        }
    }
}
