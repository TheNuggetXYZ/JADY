using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.Json;
using Avalonia;
using JADY.Models;

namespace JADY.Backend;

public static class DiaryJSON
{
    public static void Serialize(string savePath, JadySave saveInfo)
    {
        using (FileStream fs = File.Create(savePath))
        {
            string json = JsonSerializer.Serialize(saveInfo, new JsonSerializerOptions() {WriteIndented = true});
            byte[] bytes = Encoding.UTF8.GetBytes(json);
            fs.Write(bytes, 0, bytes.Length);
        }
    }
    
    public static JadySave Deserialize(string savePath)
    {
        if (!File.Exists(savePath))
            return null;
        
        using (FileStream fs = File.OpenRead(savePath))
        {
            return JsonSerializer.Deserialize<JadySave>(fs);
        }
    }
}