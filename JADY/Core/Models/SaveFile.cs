namespace JADY.Core.Models;

public class SaveFile
{
    /// <summary>
    /// Password cryptographic salt
    /// </summary>
    public byte[]? Salt { get; set; }
    
    /// <summary>
    /// This field contains save data in an encrypted format, nonce and a tag.
    /// </summary>
    public EncryptedData? EncryptedData { get; set; }
    
    /// <summary>
    /// This field is the save data in an unencrypted format.
    /// </summary>
    public SaveData? PlainData { get; set; }
}