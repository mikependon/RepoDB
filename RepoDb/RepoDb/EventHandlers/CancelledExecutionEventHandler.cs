using RepoDb.EventArguments;
using System;

namespace RepoDb.EventHandlers
{
    [Obsolete]
    internal delegate void CancelledExecutionEventHandler(object sender, CancelledExecutionEventArgs e);
}
