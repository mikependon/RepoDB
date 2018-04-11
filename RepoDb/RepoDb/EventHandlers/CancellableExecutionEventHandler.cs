using RepoDb.EventArguments;
using System;

namespace RepoDb.EventHandlers
{
    [Obsolete]
    internal delegate void CancellableExecutionEventHandler(object sender, CancellableExecutionEventArgs e);
}
