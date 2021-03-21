using BenchmarkDotNet.Columns;

namespace RepoDb.Benchmarks.SqlServer.Configurations
{
    public class BenchmarkConfigWitRows : BenchmarkConfig
    {
        public BenchmarkConfigWitRows()
        {
            AddColumn(new ParamColumn("Rows"));
        }
    }
}