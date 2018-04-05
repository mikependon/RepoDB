using RepoDb.EventArguments;
using RepoDb.EventHandlers;

namespace RepoDb
{
    public static class EventNotifier
    {
        // Before Events
        public static event CancellableExecutionEventHandler BeforeQueryExecution;
        public static event CancellableExecutionEventHandler BeforeUpdateExecution;
        public static event CancellableExecutionEventHandler BeforeDeleteExecution;
        public static event CancellableExecutionEventHandler BeforeMergeExecution;
        public static event CancellableExecutionEventHandler BeforeInsertExecution;
        public static event CancellableExecutionEventHandler BeforeBulkInsertExecution;
        public static event CancellableExecutionEventHandler BeforeExecuteNonQueryExecution;
        public static event CancellableExecutionEventHandler BeforeExecuteReaderExecution;
        public static event CancellableExecutionEventHandler BeforeExecuteReaderExExecution;
        public static event CancellableExecutionEventHandler BeforeExecuteScalarExecution;

        // After Events
        public static event ExecutionEventHandler AfterQueryExecution;
        public static event ExecutionEventHandler AfterUpdateExecution;
        public static event ExecutionEventHandler AfterDeleteExecution;
        public static event ExecutionEventHandler AfterMergeExecution;
        public static event ExecutionEventHandler AfterInsertExecution;
        public static event ExecutionEventHandler AfterBulkInsertExecution;
        public static event ExecutionEventHandler AfterExecuteNonQueryExecution;
        public static event ExecutionEventHandler AfterExecuteReaderExecution;
        public static event ExecutionEventHandler AfterExecuteReaderExExecution;
        public static event ExecutionEventHandler AfterExecuteScalarExecution;

        // Cancelled Events
        public static event CancelledExecutionEventHandler CancelledExecution;

        // Before
        internal static void OnBeforeQueryExecution(object sender, CancellableExecutionEventArgs e)
        {
            BeforeQueryExecution?.Invoke(sender, e);
        }

        internal static void OnBeforeUpdateExecution(object sender, CancellableExecutionEventArgs e)
        {
            BeforeUpdateExecution?.Invoke(sender, e);
        }

        internal static void OnBeforeDeleteExecution(object sender, CancellableExecutionEventArgs e)
        {
            BeforeDeleteExecution?.Invoke(sender, e);
        }

        internal static void OnBeforeMergeExecution(object sender, CancellableExecutionEventArgs e)
        {
            BeforeMergeExecution?.Invoke(sender, e);
        }

        internal static void OnBeforeInsertExecution(object sender, CancellableExecutionEventArgs e)
        {
            BeforeInsertExecution?.Invoke(sender, e);
        }

        internal static void OnBeforeBulkInsertExecution(object sender, CancellableExecutionEventArgs e)
        {
            BeforeBulkInsertExecution?.Invoke(sender, e);
        }

        internal static void OnBeforeExecuteNonQueryExecution(object sender, CancellableExecutionEventArgs e)
        {
            BeforeExecuteNonQueryExecution?.Invoke(sender, e);
        }

        internal static void OnBeforeExecuteReaderExecution(object sender, CancellableExecutionEventArgs e)
        {
            BeforeExecuteReaderExecution?.Invoke(sender, e);
        }

        internal static void OnBeforeExecuteReaderExExecution(object sender, CancellableExecutionEventArgs e)
        {
            BeforeExecuteReaderExExecution?.Invoke(sender, e);
        }

        internal static void OnBeforeExecuteScalarExecution(object sender, CancellableExecutionEventArgs e)
        {
            BeforeExecuteScalarExecution?.Invoke(sender, e);
        }

        // After
        internal static void OnAfterQueryExecution(object sender, ExecutionEventArgs e)
        {
            AfterQueryExecution?.Invoke(sender, e);
        }

        internal static void OnAfterUpdateExecution(object sender, ExecutionEventArgs e)
        {
            AfterUpdateExecution?.Invoke(sender, e);
        }

        internal static void OnAfterDeleteExecution(object sender, ExecutionEventArgs e)
        {
            AfterDeleteExecution?.Invoke(sender, e);
        }

        internal static void OnAfterMergeExecution(object sender, ExecutionEventArgs e)
        {
            AfterMergeExecution?.Invoke(sender, e);
        }

        internal static void OnAfterInsertExecution(object sender, ExecutionEventArgs e)
        {
            AfterInsertExecution?.Invoke(sender, e);
        }

        internal static void OnAfterBulkInsertExecution(object sender, ExecutionEventArgs e)
        {
            AfterBulkInsertExecution?.Invoke(sender, e);
        }

        internal static void OnAfterExecuteNonQueryExecution(object sender, ExecutionEventArgs e)
        {
            AfterExecuteNonQueryExecution?.Invoke(sender, e);
        }

        internal static void OnAfterExecuteReaderExecution(object sender, ExecutionEventArgs e)
        {
            AfterExecuteReaderExecution?.Invoke(sender, e);
        }

        internal static void OnAfterExecuteReaderExExecution(object sender, ExecutionEventArgs e)
        {
            AfterExecuteReaderExExecution?.Invoke(sender, e);
        }

        internal static void OnAfterExecuteScalarExecution(object sender, ExecutionEventArgs e)
        {
            AfterExecuteScalarExecution?.Invoke(sender, e);
        }

        // Cancelled

        internal static void OnCancelledExecution(object sender, CancelledExecutionEventArgs e)
        {
            CancelledExecution?.Invoke(sender, e);
        }

    }
}
