using RepoDb.EventArguments;
using System;

namespace RepoDb.EventHandlers
{
    [Obsolete]
    internal delegate void ExecutionEventHandler(object sender, ExecutionEventArgs e);
}
