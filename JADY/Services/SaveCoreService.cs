using System;
using System.IO;
using System.Text.Json;
using JADY.Models;

namespace JADY.Services;

public class SaveCoreService : ISaveCoreService
{
    public void Write(string savePath, JadySave saveInfo)
    {
        if (File.Exists(savePath))
        {
            Console.WriteLine("Save file already exists, creating backup");
            MoveOldSaveToBackup(savePath);
        }
        
        using (FileStream fs = File.Create(savePath))
        {
            JsonSerializer.Serialize(fs, saveInfo, new JsonSerializerOptions { WriteIndented = true });
            Console.WriteLine("Saved to: " + savePath);
        }
    }

    private void MoveOldSaveToBackup(string savePath)
    {
        string backupPath = savePath + ".backup";
        if (File.Exists(backupPath))
            File.Delete(backupPath);
        
        Console.WriteLine("Renaming save file to backup");
        File.Move(savePath, backupPath);
    }
    
    public JadySave Read(string savePath)
    {
        if (!File.Exists(savePath))
        {
            Console.WriteLine("Save file not found");
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
                Console.WriteLine(e);
                
                string corruptedPath = savePath + ".corrupted";
                
                Console.WriteLine("Renaming corrupted save file");
                
                if (!File.Exists(corruptedPath))
                    File.Move(savePath, corruptedPath);
                else
                {
                    Console.WriteLine($"Corrupted save file already exists: {savePath}.corrupted");
                    Console.WriteLine("Please handle your corrupted save file first");
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
            Console.WriteLine("Restoring backup");
            File.Move(backupPath, savePath);
            
            using (FileStream fs = File.OpenRead(savePath))
            {
                JadySave save = JsonSerializer.Deserialize<JadySave>(fs);
                Console.WriteLine("Restored backup");
                return save;
            }
        }
        
        Console.WriteLine("Backup file not found");
        Console.WriteLine("Creating empty save");
        
        return new();
    }
}