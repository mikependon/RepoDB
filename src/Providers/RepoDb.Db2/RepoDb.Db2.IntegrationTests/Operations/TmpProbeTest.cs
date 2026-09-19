using IBM.Data.Db2;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Db2.IntegrationTests.Setup;
using System;
using System.Collections;
using System.Linq;
using System.Text;

namespace RepoDb.Db2.IntegrationTests.Operations
{
    [TestClass]
    public class TmpProbeTest
    {
        private static readonly StringBuilder m_log = new StringBuilder();

        private static string Show(object o) =>
            o is byte[] b ? "0x" + BitConverter.ToString(b) :
            o is IEnumerable e && !(o is string) ? string.Join(",", e.Cast<object>()) : o?.ToString();

        private static void Raw(string name, string setup, string select)
        {
            try
            {
                using (var connection = new DB2Connection(Database.ConnectionString))
                {
                    connection.Open();
                    foreach (var sql in setup.Split(new[] { ";;" }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        using (var c = connection.CreateCommand()) { c.CommandText = sql; c.ExecuteNonQuery(); }
                    }
                    using (var c = connection.CreateCommand())
                    {
                        c.CommandText = select;
                        using (var r = c.ExecuteReader())
                        {
                            r.Read();
                            var v = r.GetValue(0);
                            m_log.AppendLine($"RAW  {name}: field={r.GetFieldType(0)} dbtype={r.GetDataTypeName(0)} value={v?.GetType().FullName} => {Show(v)}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                m_log.AppendLine($"RAW  {name}: FAILED {ex.GetType().Name}: {ex.Message.Split('\n')[0]}");
            }
        }

        private static void Try(string sql)
        {
            try { using (var connection = new DB2Connection(Database.ConnectionString)) { connection.Open(); using (var c = connection.CreateCommand()) { c.CommandText = sql; c.ExecuteNonQuery(); } } }
            catch (Exception) { }
        }

        [TestMethod]
        public void Probe()
        {
            Database.Initialize();
            foreach (var t in new[] { "PR_XML", "PR_DF16", "PR_DF34", "PR_ROWID", "PR_ARR", "PR_DIST", "PR_STRUCT" }) Try("DROP TABLE " + t);
            foreach (var t in new[] { "PR_INTARR", "PR_MONEY", "PR_PT" }) Try("DROP TYPE " + t);

            Raw("xml", "CREATE TABLE PR_XML (V XML);;INSERT INTO PR_XML VALUES (XMLPARSE(DOCUMENT '<a><b>1</b></a>'))", "SELECT V FROM PR_XML");
            Raw("decfloat16", "CREATE TABLE PR_DF16 (V DECFLOAT(16));;INSERT INTO PR_DF16 VALUES (1.5)", "SELECT V FROM PR_DF16");
            Raw("decfloat34", "CREATE TABLE PR_DF34 (V DECFLOAT(34));;INSERT INTO PR_DF34 VALUES (1.5)", "SELECT V FROM PR_DF34");
            Raw("decfloat_nan", "INSERT INTO PR_DF34 VALUES (DECFLOAT('NaN'))", "SELECT V FROM PR_DF34 WHERE V <> 1.5 OR V IS NAN");
            Raw("rowid", "CREATE TABLE PR_ROWID (V ROWID)", "SELECT 1 FROM PR_ROWID");
            Raw("array_column", "CREATE TYPE PR_INTARR AS INTEGER ARRAY[10];;CREATE TABLE PR_ARR (V PR_INTARR)", "SELECT 1 FROM PR_ARR");
            Raw("distinct", "CREATE DISTINCT TYPE PR_MONEY AS DECIMAL(9,2) WITH COMPARISONS;;CREATE TABLE PR_DIST (V PR_MONEY);;INSERT INTO PR_DIST VALUES (PR_MONEY(1.5))", "SELECT V FROM PR_DIST");
            Raw("structured", "CREATE TYPE PR_PT AS (X INTEGER, Y INTEGER) MODE DB2SQL;;CREATE TABLE PR_STRUCT (V PR_PT)", "SELECT 1 FROM PR_STRUCT");

            foreach (var t in new[] { "PR_XML", "PR_DF16", "PR_DF34", "PR_ROWID", "PR_ARR", "PR_DIST", "PR_STRUCT" }) Try("DROP TABLE " + t);
            foreach (var t in new[] { "PR_INTARR", "PR_MONEY", "PR_PT" }) Try("DROP TYPE " + t);
            System.IO.File.WriteAllText(@"C:\Users\micha\AppData\Local\Temp\probe.txt", m_log.ToString());
        }
    }
}
