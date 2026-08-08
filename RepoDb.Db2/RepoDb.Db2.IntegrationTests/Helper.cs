using RepoDb.Db2.IntegrationTests.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;

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
                    // Generated at exactly the declared CHAR(10)/NCHAR(10) length, so Db2's
                    // blank-padding behavior for under-length CHAR/NCHAR values never kicks in -
                    // sidesteps needing a trim-aware comparison in AssertPropertiesEquality.
                    ColumnChar = GetFixedLengthString(10),
                    ColumnNChar = GetFixedLengthString(10),

                    ColumnInt = m_random.Next(int.MinValue, int.MaxValue),
                    ColumnBigInt = ((long)m_random.Next() << 32) | (uint)m_random.Next(),
                    ColumnSmallInt = (short)m_random.Next(short.MinValue, short.MaxValue),
                    ColumnTinyInt = (byte)m_random.Next(byte.MinValue, byte.MaxValue),

                    ColumnBinaryFloat = (float)Math.Round(m_random.NextDouble() * 1000, 4),
                    ColumnBinaryDouble = Math.Round(m_random.NextDouble() * 1000, 8),

                    // WITH TIME ZONE preserves an arbitrary offset - use a non-zero one to actually
                    // exercise that. WITH LOCAL TIME ZONE always normalizes to the database's own time
                    // zone on storage/retrieval regardless of what's sent, so a UTC input keeps the
                    // round-trip comparison (which is instant-based, not offset-based, for
                    // DateTimeOffset) unambiguous.
                    ColumnTimestampTz = new DateTimeOffset(DateTime.UtcNow.Ticks, TimeSpan.Zero).ToOffset(TimeSpan.FromHours(m_random.Next(-12, 13))),
                    ColumnTimestampLtz = DateTimeOffset.UtcNow,

                    ColumnRaw = Guid.NewGuid().ToByteArray(),

                    ColumnClob = $"Clob-{new string('x', 5000)}-{m_random.Next(int.MaxValue)}",
                    ColumnNClob = $"NClob-{new string('y', 5000)}-{m_random.Next(int.MaxValue)}",
                    ColumnBlob = GetRandomBytes(5000),

                    // Millisecond-level granularity only, well within INTERVAL DAY(2) TO SECOND(6)'s
                    // microsecond precision and TimeSpan's 100ns tick resolution, avoiding any
                    // fractional-second rounding mismatch on round-trip.
                    ColumnIntervalDs = new TimeSpan(m_random.Next(0, 2), m_random.Next(0, 24), m_random.Next(0, 60), m_random.Next(0, 60), m_random.Next(0, 1000)),

                    ColumnXml = $"<Person><Name>Value-{m_random.Next(int.MaxValue)}</Name></Person>"
                };
            }
        }

        private static string GetFixedLengthString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Range(0, length).Select(_ => chars[m_random.Next(chars.Length)]).ToArray());
        }

        private static byte[] GetRandomBytes(int length)
        {
            var bytes = new byte[length];
            m_random.NextBytes(bytes);
            return bytes;
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
                    (value1, value2) = (
                        DateTime.SpecifyKind(d1, DateTimeKind.Unspecified).ToString("O", CultureInfo.InvariantCulture),
                        DateTime.SpecifyKind(d2, DateTimeKind.Unspecified).ToString("O", CultureInfo.InvariantCulture));
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
                else if (property.Name == "ColumnXml" && value1 is string xml1 && value2 is string xml2)
                {
                    // Db2's XMLTYPE storage (this project uses BINARY XML) reformats/pretty-prints
                    // the XML on storage - a compact input like "<a><b>1</b></a>" comes back with
                    // added whitespace/newlines/indentation even though the content is semantically
                    // identical. An exact string comparison would spuriously fail here, so compare
                    // the parsed XML trees instead.
                    Assert(XNode.DeepEquals(XDocument.Parse(xml1), XDocument.Parse(xml2)), property.Name, value1, value2);
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
