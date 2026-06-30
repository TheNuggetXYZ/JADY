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
    
    public record LoadResult(LoadStatus Status, SaveData? Data = null);
    
    public record ReadResult<T>(ReadStatus Status, T? Data = null) where T : class;

    public enum ReadStatus
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
    
    public LoadResult ReadSaveFromContainer(SaveFile saveFile)
    {
        if (saveFile.EncryptedData is null)
        {
            var data = saveFile.PlainData ?? CreateEmpty<SaveData>("PlainData and EncryptedData are both null.");
            return new LoadResult(LoadStatus.Success, data);
        }

        try
        {
            var decrypted = encryptionService.Decrypt(saveFile.EncryptedData, out bool correctPassword);

            if (!correctPassword)
                return new LoadResult(LoadStatus.InvalidPassword);

            var data1 = JsonSerializer.Deserialize<SaveData>(decrypted) ??
                        CreateEmpty<SaveData>("Deserializing data returned null");
            return new LoadResult(LoadStatus.Success, data1);
        }
        catch (InvalidOperationException e)
        {
            return new LoadResult(LoadStatus.InvalidPassword);
        }
    }

    public ReadResult<SaveFile> ReadSaveContainer(string path)
    {
        var save = ReadJson<SaveFile>(path);

        switch (save.Status)
        {
            case ReadStatus.FileNotFound:
                logger.LogInformation("File not found: {Path}, reading backup.", path);
                var backup = ReadJson<SaveFile>(Path.ChangeExtension(path, BackupExtension));
                return backup;
            
            case ReadStatus.Corrupted:
                logger.LogError("Corruption detected at {Path}, reading backup.", path);
                saveFsService.TryRotateFile(path, Path.ChangeExtension(path, CorruptExtension), true);
                var backup1 = ReadJson<SaveFile>(Path.ChangeExtension(path, BackupExtension));
                return new ReadResult<SaveFile>(ReadStatus.Corrupted, backup1.Data);
            
            case ReadStatus.Success when save.Data is null:
                throw new InvalidOperationException(
                    "ReadStatus is out of range or ReadResult<T>.Data is null while ReadResult<T>.Status is Success.");
        }

        return save;
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