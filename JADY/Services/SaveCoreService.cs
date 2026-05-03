using System;
using System.IO;
using System.Text.Json;
using JADY.Models;
using Microsoft.Extensions.Logging;

namespace JADY.Services;

public class SaveCoreService(ILogger<SaveCoreService> logger) : ISaveCoreService
{
    public string SavesDirectory { get; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "JADY");

    public void Write(string savePath, JadySave saveInfo)
    {
        if (File.Exists(savePath))
        {
            logger.LogInformation("Save file already exists, creating backup");
            MoveOldSaveToBackup(savePath);
        }
        
        using (FileStream fs = File.Create(savePath))
        {
            JsonSerializer.Serialize(fs, saveInfo, new JsonSerializerOptions { WriteIndented = true });
            logger.LogInformation("Saved to: " + savePath);
        }
    }

    private void MoveOldSaveToBackup(string savePath)
    {
        string backupPath = savePath + ".backup";
        if (File.Exists(backupPath))
            File.Delete(backupPath);
        
        logger.LogInformation("Renaming save file to backup");
        File.Move(savePath, backupPath);
    }
    
    public JadySave Read(string savePath)
    {
        if (!File.Exists(savePath))
        {
            logger.LogWarning("Save file not found");
            return DeserializeBackup(savePath);
        }
        
        using (FileStream fs = File.OpenRead(savePath))
        {
            try 
            {
                return JsonSerializer.Deserialize<JadySave>(fs);
            }
            catch (JsonException e)
            {
                logger.LogTrace(e, "Error deserializing save file");
                
                string corruptedPath = savePath + ".corrupted";
                
                logger.LogInformation("Renaming corrupted save file");
                
                if (!File.Exists(corruptedPath))
                    File.Move(savePath, corruptedPath);
                else
                {
                    logger.LogError($"Corrupted save file already exists: {savePath}.corrupted. Please handle your corrupted save file first");
                    throw;
                }
                
                return DeserializeBackup(savePath);
            }
        }
    }

    private JadySave DeserializeBackup(string savePath)
    {
        string backupPath = savePath + ".backup";
        if (File.Exists(backupPath))
        {
            logger.LogInformation("Restoring backup");
            File.Move(backupPath, savePath);
            
            using (FileStream fs = File.OpenRead(savePath))
            {
                JadySave save = JsonSerializer.Deserialize<JadySave>(fs);
                logger.LogInformation("Restored backup");
                return save;
            }
        }
        
        logger.LogWarning("Backup file not found");
        logger.LogInformation("Creating empty save");
        
        return new();
    }
}