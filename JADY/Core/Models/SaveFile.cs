namespace JADY.Core.Models;

public class SaveFile
{
    /// <summary>
    /// Password cryptographic salt
    /// </summary>
    public byte[]? Salt { get; init; }
    
    /// <summary>
    /// This field contains save data in an encrypted format, nonce and a tag.
    /// </summary>
    public EncryptedData? EncryptedData { get; init; }
    
    /// <summary>
    /// This field is the save data in an unencrypted format.
    /// </summary>
    public SaveData? PlainData { get; init; }
}