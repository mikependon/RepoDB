using System;
using RepoDb.Interfaces;
using RepoDb.Options;

namespace RepoDb.MySqlConnector.PropertyHandlers
{
    public class BoolPropertyHandler : IPropertyHandler<SByte, bool>
    {
        public bool Get(SByte input, PropertyHandlerGetOptions options)
        {
            return input != 0;
        }

        public SByte Set(bool input, PropertyHandlerSetOptions options)
        {
            return input ? (SByte)1 : (SByte)0;
        }
    }
}
