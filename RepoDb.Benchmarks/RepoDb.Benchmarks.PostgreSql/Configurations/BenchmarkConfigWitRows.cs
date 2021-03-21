using BenchmarkDotNet.Columns;

namespace RepoDb.Benchmarks.PostgreSql.Configurations
{
    public class BenchmarkConfigWitRows : BenchmarkConfig
    {
        public BenchmarkConfigWitRows()
        {
            AddColumn(new ParamColumn("Rows"));
        }
    }
}