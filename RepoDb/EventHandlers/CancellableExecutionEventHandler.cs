using RepoDb.EventArguments;

namespace RepoDb.EventHandlers
{
    public delegate void CancellableExecutionEventHandler(object sender, CancellableExecutionEventArgs e);
}
