using System;
using System.IO;
using System.Text.Json;
using JADY.Core.Models;
using Microsoft.Extensions.Logging;

namespace JADY.Services;

public class SaveCoreService(ILogger<SaveCoreService> logger, IEncryptionService encryptionService) : ISaveCoreService
{
    public string SavesDirectory { get; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "JADY");
    
    public bool ExistsFile(string path) => File.Exists(path);
    
    public void Write(string path, Config config)
    {
        WriteJson(path, config, true);
    }
    
    public void Write(string path, SaveData saveData)
    {
        if (File.Exists(path))
        {
            logger.LogInformation("Save file already exists, creating backup");
            CreateBackupFrom(path);
        }
        
        string jsonString = JsonSerializer.Serialize(saveData);

        WriteJson(path, encryptionService.Encrypt(jsonString), true);
    }

    public Config ReadConfig(string path)
    {
        if (!File.Exists(path))
            return CreateEmpty<Config>("Config file not found");

        try
        {
            return ReadExistingFile<Config>(path);
        }
        catch (JsonException e)
        {
            logger.LogTrace(e, "Error deserializing config file:");
            return CreateEmpty<Config>("Error deserializing config file");
        }
    }
    
    public SaveData ReadSave(string path)
    {
        if (!File.Exists(path))
        {
            logger.LogWarning("Save file not found");
            
            return RestoreBackup(path);
        }
        
        try
        {
            return ReadExistingSaveFile(path, true);
        }
        catch (JsonException e)
        {
            logger.LogTrace(e, "Error deserializing save file");
                
            CreateCorruptedFrom(path, path + ".corrupted");

            return RestoreBackup(path);
        }
    }
    
    
    
    private void CreateBackupFrom(string path)
    {
        string backupPath = path + ".backup";
        
        if (File.Exists(backupPath))
            File.Delete(backupPath);
        
        logger.LogInformation($"Creating backup file from {path}");
        File.Move(path, backupPath);
    }

    private void CreateCorruptedFrom(string path, string corruptedPath)
    {
        logger.LogInformation("Renaming corrupted save file");

        if (!File.Exists(corruptedPath))
            File.Move(path, corruptedPath);
        else
        {
            logger.LogError($"Corrupted save file already exists: {path}.corrupted. Please handle your corrupted save file first");
            throw new InvalidOperationException($"Corrupted save file already exists: {path}.corrupted. Please handle your corrupted save file first");
        }
    }

    private SaveData RestoreBackup(string path)
    {
        string backupPath = path + ".backup";
        
        logger.LogInformation("Restoring backup...");
        
        if (!File.Exists(backupPath)) 
            return CreateEmpty<SaveData>("Backup file not found");
        
        File.Move(backupPath, path);
        
        return ReadExistingSaveFile(path);
    }
    
    private SaveData ReadExistingSaveFile(string path, bool throwException = false)
    {
        try
        {
            using FileStream fs = File.OpenRead(path);
            var data = JsonSerializer.Deserialize<EncryptedData>(fs);
            
            if (data != null)
                return JsonSerializer.Deserialize<SaveData>(encryptionService.Decrypt(data, out _))!;
        }
        catch (JsonException e)
        {
            logger.LogTrace(e, "Error deserializing save file");
            
            if (throwException)
                throw;
                
            return CreateEmpty<SaveData>("Error deserializing save file");
        }
        
        return CreateEmpty<SaveData>("Reading file returned null");
    }
    
    
    
    private void WriteJson<T>(string path, T obj, bool writeIndented)
    {
        using FileStream fs = File.Create(path);
        
        JsonSerializer.Serialize(fs, obj, new JsonSerializerOptions { WriteIndented = writeIndented });
        logger.LogInformation("Saved to: " + path);
    }

    private T ReadExistingFile<T>(string path) where T : new()
    {
        using FileStream fs = File.OpenRead(path);
        var data = JsonSerializer.Deserialize<T>(fs);
        
        return data ?? CreateEmpty<T>("Reading file returned null");
    }

    private T CreateEmpty<T>(string reason) where T : new()
    {
        logger.LogInformation($"{reason} -> Creating empty {nameof(T)}");
        return new T();
    }
}