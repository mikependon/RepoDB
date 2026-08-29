namespace System.Runtime.CompilerServices
{
#if !NET
    // Required to allow init properties in netstandard
    internal sealed class IsExternalInit : Attribute
    {
    }
#endif
}
