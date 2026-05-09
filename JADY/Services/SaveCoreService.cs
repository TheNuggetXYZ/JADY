using System;
using System.IO;
using System.Text.Json;
using JADY.Core.Models;
using Microsoft.Extensions.Logging;

namespace JADY.Services;

public class SaveCoreService(ILogger<SaveCoreService> logger, IEncryptionService encryptionService) : ISaveCoreService
{
    public string SavesDirectory { get; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "JADY");

    public void Write(string path, SaveData saveData)
    {
        if (File.Exists(path))
        {
            logger.LogInformation("Save file already exists, creating backup");
            MoveOldSaveToBackup(path);
        }
        
        string jsonString = JsonSerializer.Serialize(saveData);

        using FileStream fs = File.Create(path);
        
        JsonSerializer.Serialize(fs, encryptionService.Encrypt(jsonString), new JsonSerializerOptions{ WriteIndented = true });
        
        logger.LogInformation("Saved to: " + path);
    }
    
    public void Write(string path, Config config)
    {
        using FileStream fs = File.Create(path);
        
        JsonSerializer.Serialize(fs, config, new JsonSerializerOptions { WriteIndented = true });
        logger.LogInformation("Saved to: " + path);
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

    public bool ExistsSave(string savePath)
    {
        return File.Exists(savePath);
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