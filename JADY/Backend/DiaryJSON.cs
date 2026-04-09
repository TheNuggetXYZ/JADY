using System;
using System.IO;
using System.Text;
using System.Text.Json;
using Avalonia;
using JADY.Models;

namespace JADY.Backend;

public static class DiaryJSON
{
    public static JADYSave JadySave { get; private set; } = new();
    
    public static void Save(Settings settings)
    {
        JadySave.Settings = settings;
        
        Save();
    }
    
    public static void Save(Diary[] diaries)
    {
        JadySave.Diaries = diaries;
        
        Save();
    }

    private static void Save()
    {
        string savePath = GetSavePath();
        
        using (FileStream fs = File.Create(savePath))
        {
            string json = JsonSerializer.Serialize(JadySave, new JsonSerializerOptions() {WriteIndented = true});
            byte[] bytes = Encoding.UTF8.GetBytes(json);
            fs.Write(bytes, 0, bytes.Length);
        }
    }
    
    public static void Load()
    {
        string savePath = GetSavePath();

        if (!File.Exists(savePath))
            return;
        
        using (FileStream fs = File.OpenRead(savePath))
        {
            JadySave = JsonSerializer.Deserialize<JADYSave>(fs);
        }
    }

    private static string GetSavePath()
    {
        return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "JADY.save");
    }
}

[Serializable]
public class JADYSave
{
    public Settings Settings { get; set; } = new();
    public Diary[] Diaries {get; set;} = [];
}