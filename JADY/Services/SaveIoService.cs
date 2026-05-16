using System;
using System.IO;
using System.Text.Json;
using JADY.Core.Data;
using JADY.Core.Models;
using Microsoft.Extensions.Logging;

namespace JADY.Services;

public class SaveIoService(ILogger<SaveIoService> logger, ISaveFsService saveFsService, IEncryptionService encryptionService) : ISaveIoService
{
    public string SavesDirectory { get; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "JADY");
    
    private const string SaveExtension = ".save";
    private const string BackupExtension = ".backup";
    private const string CorruptExtension = ".corrupted";
    
    public record LoadResult(LoadStatus Status, SaveFile? Container = null, SaveData? Data = null);
    
    private record ReadResult<T>(ReadStatus Status, T? Data = null) where T : class;

    private enum ReadStatus
    {
        Success = 0,
        FileNotFound = 1,
        Corrupted = 2,
    }
    
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

        return ReadJson<Config>(path).Data ?? CreateEmpty<Config>("Error reading config");
    }
    
    public LoadResult ReadSave(string path)
    {
        var result = TryReadAndExtractSaveData(path);

        if (result.Status is LoadStatus.FileNotFound or LoadStatus.Corrupted)
        {
            logger.LogWarning("Reading save file failed or it is missing. Restoring backup...");
            
            if (saveFsService.TryRotateFile(Path.ChangeExtension(path, BackupExtension), path, true)) // TODO: Use Copy instead of Rotate/Move here to keep the backup as a safety net
            {
                result = TryReadAndExtractSaveData(path);
            }
        }

        return result;
    }

    public LoadResult ReadSaveContainer(string path)
    {
        var save = ReadJson<SaveFile>(path);

        switch (save.Status)
        {
            case ReadStatus.Success when save.Data is not null:
                return new LoadResult(LoadStatus.Success, save.Data);
                
            case ReadStatus.Corrupted:
            {
                logger.LogError("Corruption detected at {Path}", path);
                saveFsService.TryRotateFile(path, Path.ChangeExtension(path, CorruptExtension), true);
                return new LoadResult(LoadStatus.Corrupted);
            }
            
            case ReadStatus.FileNotFound:
                return new LoadResult(LoadStatus.FileNotFound);
            
            default:
                throw new InvalidOperationException("ReadStatus is out of range or ReadResult<T>.Data is null while ReadResult<T>.Status is Success.");
        }
    }

    private LoadResult TryReadAndExtractSaveData(string path)
    {
        var save = ReadJson<SaveFile>(path);

        switch (save.Status)
        {
            case ReadStatus.Success when save.Data is not null:
                return ExtractSaveData(save.Data);
                
            case ReadStatus.Corrupted:
            {
                logger.LogError("Corruption detected at {Path}", path);
                saveFsService.TryRotateFile(path, Path.ChangeExtension(path, CorruptExtension), true);
                return new LoadResult(LoadStatus.Corrupted);
            }
            
            case ReadStatus.FileNotFound:
                return new LoadResult(LoadStatus.FileNotFound);
            
            default:
                throw new InvalidOperationException("ReadStatus is out of range or ReadResult<T>.Data is null while ReadResult<T>.Status is Success.");
        }
    }

    private LoadResult ExtractSaveData(SaveFile saveFile)
    {
        if (saveFile.EncryptedData is null)
        {
            var data = saveFile.PlainData ?? CreateEmpty<SaveData>("PlainData and EncryptedData are both null.");
            return new LoadResult(LoadStatus.Success, saveFile, data);
        }

        var decrypted = encryptionService.Decrypt(saveFile.EncryptedData, out bool correctPassword);

        if (!correctPassword)
            return new LoadResult(LoadStatus.InvalidPassword, saveFile);
        
        var data1 = JsonSerializer.Deserialize<SaveData>(decrypted) ?? CreateEmpty<SaveData>("Deserializing data returned null");
        return new LoadResult(LoadStatus.Success, saveFile, data1);
    }
    
    private void WriteJson<T>(string path, T obj)
    {
        logger.LogInformation("Saving to: {Path}", path);
        
        using var stream = saveFsService.OpenWrite(path);
        JsonSerializer.Serialize(stream, obj, new JsonSerializerOptions{WriteIndented = true});
    }

    /// <exception cref="JsonException">Is thrown if the JSON file is incorrectly edited</exception>
    private ReadResult<T> ReadJson<T>(string path) where T : class
    {
        logger.LogInformation("Reading: {Path}", path);

        if (!File.Exists(path))
            return new ReadResult<T>(ReadStatus.FileNotFound);
        
        try
        {
            using var stream = saveFsService.OpenRead(path);
            return new ReadResult<T>(ReadStatus.Success, JsonSerializer.Deserialize<T>(stream));
        }
        catch (JsonException e)
        {
            return new ReadResult<T>(ReadStatus.Corrupted);
        }
    }

    private T CreateEmpty<T>(string reason) where T : new()
    {
        logger.LogInformation($"{reason} -> Creating empty {typeof(T)}");
        return new T();
    }
}