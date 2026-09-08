using System;
using System.IO;

namespace JADY.Core.Data;

public static class AppDirectories
{
    public static string DataDirectory { get; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "JADY");
    public static string SavesDirectory { get; } = Path.Combine(DataDirectory, "Saves");
    public static string FileAttachmentDirectory { get; } = Path.Combine(DataDirectory, "FileAttachments");
}