using System;

namespace JADY.Models;

[Serializable]
public class JadySave
{
    public Settings Settings { get; set; } = new();
    public Diary[] Diaries {get; set;} = [];

    public JadySave()
    {
        Settings.SaveFilePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
    }
}