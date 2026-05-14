namespace JADY.Services;

public interface ISaveFSService
{
    void RotateFile(string path, string newPath);
}