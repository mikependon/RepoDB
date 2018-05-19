using System;
using System.Reflection;

namespace RepoDb.Interfaces
{
    public interface ITraceLog
    {
        MethodBase Method { get; }
        object Result { get; }
        object Parameter { get; set; }
        string Statement { get; set; }
        TimeSpan ExecutionTime { get; }
    }
}