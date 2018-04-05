using RepoDb.EventArguments;

namespace RepoDb.EventHandlers
{
    public delegate void CancelledExecutionEventHandler(object sender, CancelledExecutionEventArgs e);
}
