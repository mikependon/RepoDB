using System;
using System.Diagnostics;

namespace RepoDb
{
    public static class PerformanceMonitor
    {
        // Privates
        private static Stopwatch m_watch = new Stopwatch();
        private static int m_count = 0;

        // Methods
        public static void Reset()
        {
            m_watch.Reset();
            m_count = 0;
        }

        public static void Start()
        {
            m_watch.Start();
        }

        public static void Stop()
        {
            m_watch.Stop();
        }

        public static TimeSpan GetElapsedTime()
        {
            return m_watch.Elapsed;
        }

        public static void Count()
        {
            m_count++;
        }

        public static int GetCount()
        {
            return m_count;
        }
    }
}
