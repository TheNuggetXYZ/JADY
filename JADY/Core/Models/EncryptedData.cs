namespace JADY.Core.Models;

public sealed class EncryptedData
{
    public required byte[] Data { get; init; }

    public required byte[] Nonce { get; init; }

    public required byte[] Tag { get; init; }
}