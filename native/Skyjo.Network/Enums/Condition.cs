using Metalama.Framework.Aspects;

namespace Skyjo.Network.Enums;

[CompileTime]
public enum Condition
{
    Default,
    SkipOwner,
    OwnerOnly
}