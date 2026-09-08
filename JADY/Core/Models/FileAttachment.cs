using System;

namespace JADY.Core.Models;

public class FileAttachment
{
    public string? OriginalFileName { get; init; }
    
    public ulong? FileSizeBytes { get; init; }
    
    public DateTimeOffset DateAdded { get; init; }
    
    public readonly string GuidString = Guid.NewGuid().ToString("N");
}