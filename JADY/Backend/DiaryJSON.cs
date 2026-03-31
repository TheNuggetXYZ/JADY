using System;
using System.IO;
using System.Text;
using System.Text.Json;
using Avalonia;
using JADY.Models;

namespace JADY.Backend;

public static class DiaryJSON
{
    private static JADYSave _jadySave = new();
    
    public static void Save(Diary[] diaries)
    {
        string savePath = GetSavePath();
        
        // Create the file, or overwrite if the file exists.
        using (FileStream fs = File.Create(savePath))
        {
            _jadySave.Diaries = diaries;
            string json = JsonSerializer.Serialize(_jadySave, new JsonSerializerOptions() {WriteIndented = true});
            byte[] bytes = Encoding.UTF8.GetBytes(json);
            fs.Write(bytes, 0, bytes.Length);
        }
    }

    public static Diary[] Load()
    {
        string savePath = GetSavePath();
        return null;
    }

    private static string GetSavePath()
    {
        return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "JADY.save");
    }
}

[Serializable]
public class JADYSave
{
    public Diary[] Diaries {get; set;} = [];
}