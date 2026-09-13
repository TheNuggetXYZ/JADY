namespace JADY.Core.Helpers;

public static class BytesUnitConverter
{
    private static readonly string[] Units = ["B", "KB", "MB", "GB", "TB", "PB"];
    
    public static string BytesToHuman(ulong bytes)
    {
        for (int i = 0; i < Units.Length; i++)
        {
            if (bytes < 1000)
                return bytes + " " + Units[i];
            
            bytes /= 1000;
        }
        
        return bytes + " " + Units[^1];
    }
}