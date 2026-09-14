namespace JADY.Core.Helpers;

public static class BytesUnitConverter
{
    private static readonly string[] Units = ["B", "KB", "MB", "GB", "TB", "PB"];
    
    public static string BytesToHuman(ulong bytes)
    {
        double size = bytes;
        
        for (int i = 0; i < Units.Length - 1; i++)
        {
            if (bytes < 1000)
                return bytes + " " + Units[i];
            if (size < 1000)
                return $"{size:F} " + Units[i];
            
            size /= 1000;
        }
        
        return $"{size:F} " + Units[^1];
    }
}