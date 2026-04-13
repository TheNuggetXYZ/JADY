using System;
using System.Globalization;
using System.IO;
using JADY.Models;

namespace JADY.Backend;

public static class Saves
{
    public static JadySave JadySave { get; private set; } = new();
    
    public static event Action OnSaveChanged = delegate {};
    
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
        
        OnSaveChanged();
    }

    public static void Load()
    {
        JadySave = DiaryJSON.Deserialize(GetSavePath());
        JadySave.Load();

        OnSaveChanged();
    }
    
    private static string GetSavePath()
    {
        if (JadySave.Settings.SaveFilePath != null)
            return Path.Combine(JadySave.Settings.SaveFilePath, "JADY.save");
        
        throw new DirectoryNotFoundException("JADY save file path not found");
    }
}