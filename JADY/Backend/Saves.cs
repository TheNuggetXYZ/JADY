using System.Globalization;
using System.IO;
using CommunityToolkit.Mvvm.Messaging;
using JADY.Models;
using Microsoft.Extensions.DependencyInjection;

namespace JADY.Backend;

public static class Saves
{
    public static JadySave JadySave { get; private set; } = new();
    
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
        
        WeakReferenceMessenger.Default.Send(new Messages.AnySaveMessage());
        WeakReferenceMessenger.Default.Send(new Messages.SaveChangeMessage());
    }

    public static void Load()
    {
        JadySave = DiaryJSON.Deserialize(GetSavePath());
        JadySave.Load();

        if (App.ServiceProvider is not null)
            App.ServiceProvider.GetRequiredService<IAppVisualService>().SetTheme(JadySave.Settings.IsThemeDark);

        WeakReferenceMessenger.Default.Send(new Messages.SaveChangeMessage());
    }
    
    private static string GetSavePath()
    {
        if (JadySave.Settings.SaveFilePath != null)
            return Path.Combine(JadySave.Settings.SaveFilePath, "JADY.save");
        
        throw new DirectoryNotFoundException("JADY save file path not found");
    }
}