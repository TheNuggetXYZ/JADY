namespace JADY.Models;

public class JadySave
{
    public Settings Settings { get; set; } = new();
    public Diary[] Diaries {get; set;} = [];

    public void Load()
    {
        Settings.Load();
    }
}