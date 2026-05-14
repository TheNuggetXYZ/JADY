using System.IO;
using Microsoft.Extensions.Logging;

namespace JADY.Services;

public class SaveFsService(ILogger<SaveFsService> logger) : ISaveFsService
{
    public bool TryRotateFile(string source, string destination, bool overwrite)
    {
        if (!File.Exists(source))
            return false;
        
        logger.LogInformation("Rotating file from {Path} to {NewPath}", source, destination);

        if (File.Exists(destination) && !overwrite) 
            return false;
        
        File.Delete(destination);
        File.Move(source, destination);
        return true;

    }
}