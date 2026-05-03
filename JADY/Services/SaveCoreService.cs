using System;
using System.IO;
using System.Text.Json;
using JADY.Core.Models;
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

        using FileStream fs = File.Create(savePath);
        
        JsonSerializer.Serialize(fs, saveInfo, new JsonSerializerOptions { WriteIndented = true });
        logger.LogInformation("Saved to: " + savePath);
    }
    
    public void Write(string savePath, Settings config)
    {
        if (File.Exists(savePath))
        {
            logger.LogInformation("Config file already exists, creating backup");
            MoveOldSaveToBackup(savePath);
        }

        using FileStream fs = File.Create(savePath);
        
        JsonSerializer.Serialize(fs, config, new JsonSerializerOptions { WriteIndented = true });
        logger.LogInformation("Saved to: " + savePath);
    }

    private void MoveOldSaveToBackup(string savePath)
    {
        string backupPath = savePath + ".backup";
        if (File.Exists(backupPath))
            File.Delete(backupPath);
        
        logger.LogInformation("Renaming save file to backup");
        File.Move(savePath, backupPath);
    }

    public T Read<T>(string savePath) where T : new()
    {
        if (!File.Exists(savePath))
        {
            logger.LogWarning("Save file not found");
            
            var backup = DeserializeBackup<T>(savePath);

            if (backup is not null) 
                return backup;

            logger.LogInformation("Reading file returned null. Creating empty save.");
            return new T();
        }

        using FileStream fs = File.OpenRead(savePath);
        
        try
        {
            var save = JsonSerializer.Deserialize<T>(fs);

            if (save is not null)
                return save;
            
            logger.LogInformation("Reading file returned null. Creating empty save.");
            return new T();
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
                
            var backup = DeserializeBackup<T>(savePath);

            if (backup is not null) 
                return backup;

            logger.LogInformation("Reading file returned null. Creating empty save.");
            return new T();
        }
    }

    private T? DeserializeBackup<T>(string savePath) where T : new()
    {
        string backupPath = savePath + ".backup";
        if (File.Exists(backupPath))
        {
            logger.LogInformation("Restoring backup");
            File.Move(backupPath, savePath);

            using FileStream fs = File.OpenRead(savePath);
            
            var save = JsonSerializer.Deserialize<T>(fs);
            logger.LogInformation("Restored backup");
            return save;
        }
        
        logger.LogWarning("Backup file not found");
        logger.LogInformation("Creating empty save");
        
        return new T();
    }
}