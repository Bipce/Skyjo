namespace Skyjo.Network.Enums;

public enum PacketType : byte
{
    CreateEntity,
    Rpc,
    DestroyEntity,
    Replicated
}