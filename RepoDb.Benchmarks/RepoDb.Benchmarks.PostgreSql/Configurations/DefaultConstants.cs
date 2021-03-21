namespace RepoDb.Benchmarks.PostgreSql.Configurations
{
    public class DefaultConstants
    {
        /// <summary>
        /// How many times we should launch process with target benchmark.
        /// </summary>
        public const int DefaultLaunchCount = 1;

        /// <summary>
        /// How many warmup iterations should be performed.
        /// </summary>
        public const int DefaultWarmupCount = 2;

        /// <summary>
        /// How many times the benchmark method will be invoked per one iteration of a generated loop.
        /// </summary>
        public const int DefaultUnrollFactor = 500;

        /// <summary>
        /// How many target iterations should be performed.
        /// </summary>
        public const int DefaultIterationCount = 10;
    }
}