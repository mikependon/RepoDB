using BenchmarkDotNet.Columns;

namespace RepoDb.Benchmarks.Configurations
{
    public class BenchmarkConfigWitRows : BenchmarkConfig
    {
        public BenchmarkConfigWitRows()
        {
            AddColumn(new ParamColumn("Rows"));
        }
    }
}