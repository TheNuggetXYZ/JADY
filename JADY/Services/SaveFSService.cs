using System;
using System.IO;
using JADY.Core.Models;
using Microsoft.Extensions.Logging;

namespace JADY.Services;

public class SaveFSService(ILogger<SaveFSService> logger) : ISaveFSService
{
    public void RestoreFile(string path, string newPath)
    {
        logger.LogInformation("Restoring file from {Path} to {NewPath}", path, newPath);
        
        File.Move(path, newPath);
    }

    public void RotateFile(string path, string newPath)
    {
        logger.LogInformation("Rotating file from {Path} to {NewPath}", path, newPath);
        
        // Delete old rotation
        if (File.Exists(newPath))
            File.Delete(newPath);
        
        File.Move(path, newPath);
    }
}