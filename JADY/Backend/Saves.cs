using System;
using System.Globalization;
using System.IO;
using JADY.Models;

namespace JADY.Backend;

public static class Saves
{
    public static JadySave JadySave { get; private set; } = new();
    
    public static Action? OnSaveChanged;
    
    public static void Save(Settings settings)
    {
        JadySave.Settings = settings;
        JadySave.Settings.CultureInfo = new CultureInfo(settings.CultureInfoName);
        
        Save();
    }
    
    public static void Save(Diary[] diaries)
    {
        JadySave.Diaries = diaries;
        
        Save();
    }

    private static void Save()
    {
        DiaryJSON.Serialize(GetSavePath(), JadySave);
        
        OnSaveChanged?.Invoke();
    }

    public static void Load()
    {
        JadySave = DiaryJSON.Deserialize(GetSavePath());
        
        OnSaveChanged?.Invoke();
    }
    
    private static string GetSavePath()
    {
        if (JadySave.Settings.SaveFilePath != null)
            return Path.Combine(JadySave.Settings.SaveFilePath, "JADY.save");
        
        throw new DirectoryNotFoundException("JADY save file path not found");
    }
}