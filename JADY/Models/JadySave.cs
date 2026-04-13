using System;

namespace JADY.Models;

[Serializable]
public class JadySave
{
    public Settings Settings { get; set; } = new();
    public Diary[] Diaries {get; set;} = [];

    public void Load()
    {
        Settings.Load();
    }
}