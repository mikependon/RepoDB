using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Interfaces;
using Serilog;

namespace RepoDb.Telemetry.Core.UnitTests.Traces
{
    [TestClass]
    public class TelemetryTraceTest
    {
        #region Classes

        // CancellableTraceLog/ResultTraceLog<T> only expose a 'protected internal' constructor, which
        // is only reachable from a derived class (regardless of assembly). These test doubles exist
        // solely to construct instances so TelemetryTrace's public ITrace methods can be exercised
        // directly, without needing to run a full DbConnection operation pipeline.
        private sealed class InspectableCancellableTraceLog : CancellableTraceLog
        {
            public InspectableCancellableTraceLog(Guid sessionId,
                string key,
                string statement,
                IEnumerable<IDbDataParameter> parameters = null)
                : base(sessionId, key, statement, parameters)
            { }
        }

        private sealed class InspectableResultTraceLog<TResult> : ResultTraceLog<TResult>
        {
            public InspectableResultTraceLog(Guid sessionId,
                string key,
                TimeSpan? executionTime,
                TResult result,
                CancellableTraceLog beforeExecutionLog)
                : base(sessionId, key, executionTime, result, beforeExecutionLog)
            { }
        }

        // TelemetryTrace and TelemetryPublisherRepository are both abstract, so they cannot be
        // instantiated directly. These minimal test doubles exist solely so the base class'
        // behavior (BeforeExecution/AfterExecution/Start, etc.) can be exercised directly.
        private sealed class InspectableTelemetryTrace : TelemetryTrace
        {
            public InspectableTelemetryTrace(
                TelemetryOption option,
                Action<Exception> errorCallback = null,
                ILogger logger = null)
                : base(option, errorCallback, logger)
            { }

            public override TelemetryPublisherRepository GetPublisherRepository()
                => new InspectableTelemetryPublisherRepository();
        }

        private sealed class InspectableTelemetryPublisherRepository : TelemetryPublisherRepository
        {
            public override string GetRequestUri() => "v1/telemetry/test";
        }

        #endregion

        [TestMethod]
        public void TestTelemetryTraceImplementsITrace()
        {
            // Act
            var trace = new InspectableTelemetryTrace(new TelemetryOption("MyApplication"));
            var actual = trace is ITrace;
            var expected = true;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestTelemetryTraceStartDoesNotThrow()
        {
            // Act
            var trace = new InspectableTelemetryTrace(new TelemetryOption("MyApplication"));
            trace.Start();
            var actual = true;
            var expected = true;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestTelemetryTraceBeforeExecutionDoesNotCancelTheOperation()
        {
            // Act
            var trace = new InspectableTelemetryTrace(new TelemetryOption("MyApplication"));
            var log = new InspectableCancellableTraceLog(Guid.NewGuid(), "Query", "SELECT 1;");
            trace.BeforeExecution(log);
            var actual = log.IsCancelled;
            var expected = false;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestTelemetryTraceBeforeExecutionAsyncReturnsCompletedTask()
        {
            // Act
            var trace = new InspectableTelemetryTrace(new TelemetryOption("MyApplication"));
            var log = new InspectableCancellableTraceLog(Guid.NewGuid(), "Query", "SELECT 1;");
            var actual = trace.BeforeExecutionAsync(log).IsCompletedSuccessfully;
            var expected = true;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestTelemetryTraceAfterExecutionDoesNotThrowForAValidLog()
        {
            // Act
            var trace = new InspectableTelemetryTrace(new TelemetryOption("MyApplication"));
            var beforeLog = new InspectableCancellableTraceLog(Guid.NewGuid(), "Query", "SELECT 1;");
            var resultLog = new InspectableResultTraceLog<int>(beforeLog.SessionId, "Query", TimeSpan.FromMilliseconds(5), 1, beforeLog);
            trace.AfterExecution(resultLog);
            var actual = resultLog.Result;
            var expected = 1;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestTelemetryTraceAfterExecutionAsyncReturnsCompletedTask()
        {
            // Act
            var trace = new InspectableTelemetryTrace(new TelemetryOption("MyApplication"));
            var beforeLog = new InspectableCancellableTraceLog(Guid.NewGuid(), "Query", "SELECT 1;");
            var resultLog = new InspectableResultTraceLog<int>(beforeLog.SessionId, "Query", TimeSpan.FromMilliseconds(5), 1, beforeLog);
            var actual = trace.AfterExecutionAsync(resultLog).IsCompletedSuccessfully;
            var expected = true;

            // Assert
            Assert.AreEqual(expected, actual);
        }
    }
}
