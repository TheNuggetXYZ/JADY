using System;
using System.IO;
using System.Text.Json;
using JADY.Core.Models;
using Microsoft.Extensions.Logging;

namespace JADY.Services;

public class SaveIoService(ILogger<SaveIoService> logger, ISaveFsService saveFsService, IEncryptionService encryptionService) : ISaveIoService
{
    public string SavesDirectory { get; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "JADY");
    
    private const string SaveExtension = ".save";
    private const string BackupExtension = ".backup";
    private const string CorruptExtension = ".corrupted";
    
    public bool ExistsFile(string path) => File.Exists(path);
    
    public void Write(string path, Config config)
    {
        WriteJson(path, config);
    }
    
    public void Write(string path, SaveData saveData, SaveFile saveFile)
    {
        // Prepare data
        var json = JsonSerializer.Serialize(saveData);
        var encrypted = encryptionService.Encrypt(json);

        saveFile.EncryptedData = encrypted;
        saveFile.PlainData = encrypted == null ? saveData : null;

        // Move existing to a backup, then write new
        saveFsService.TryRotateFile(path, Path.ChangeExtension(path, BackupExtension), true);

        WriteJson(path, saveFile);
    }

    public Config ReadConfig(string path)
    {
        if (!File.Exists(path))
            return CreateEmpty<Config>("Config file not found");

        try
        {
            return ReadJson<Config>(path) ?? CreateEmpty<Config>("Reading file returned null");
        }
        catch (JsonException e)
        {
            logger.LogTrace(e, "Error deserializing config file:");
            return CreateEmpty<Config>("Error deserializing config file");
        }
    }
    
    public SaveData ReadSave(string path)
    {
        var result = TryReadAndExtractSaveData(path);

        if (result is null)
        {
            logger.LogWarning("Reading save file failed or it is missing. Restoring backup...");
            
            if (saveFsService.TryRotateFile(Path.ChangeExtension(path, BackupExtension), path, true)) // TODO: Use Copy instead of Rotate/Move here to keep the backup as a safety net
            {
                result = TryReadAndExtractSaveData(path);
            }
        }

        return result ?? CreateEmpty<SaveData>("All recovery attempts failed.");
    }

    private SaveData? TryReadAndExtractSaveData(string path)
    {
        try
        {
            var save = ReadJson<SaveFile>(path);

            return save is not null
                ? ExtractSaveData(save)
                : null;
        }
        catch (JsonException e)
        {
            logger.LogError(e, "Corruption detected at {Path}", path);
            saveFsService.TryRotateFile(path, Path.ChangeExtension(path, CorruptExtension), true);
            return null;
        }
    }

    private SaveData ExtractSaveData(SaveFile saveFile)
    {
        if (saveFile.EncryptedData is null)
            return saveFile.PlainData ?? 
                   CreateEmpty<SaveData>("PlainData and EncryptedData are both null.");

        var decrypted = encryptionService.Decrypt(saveFile.EncryptedData, out bool correctPassword);

        //TODO: Resolve correctPassword
        
        return JsonSerializer.Deserialize<SaveData>(decrypted) ??
               CreateEmpty<SaveData>("Error deserializing EncryptedData");
    }

    private T CreateEmpty<T>(string reason) where T : new()
    {
        logger.LogInformation($"{reason} -> Creating empty {typeof(T)}");
        return new T();
    }
    
    private void WriteJson<T>(string path, T obj)
    {
        logger.LogInformation("Saving to: {Path}", path);
        
        using var stream = saveFsService.OpenWrite(path);
        JsonSerializer.Serialize(stream, obj, new JsonSerializerOptions{WriteIndented = true});
    }

    /// <exception cref="JsonException">Is thrown if the JSON file is incorrectly edited</exception>
    private T? ReadJson<T>(string path) where T : class
    {
        logger.LogInformation("Reading: {Path}", path);

        if (!File.Exists(path))
            return null;
        
        using var stream = saveFsService.OpenRead(path);
        return JsonSerializer.Deserialize<T>(stream);
    }
}