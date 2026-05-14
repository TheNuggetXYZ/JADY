namespace JADY.Services;

public interface ISaveFsService
{
    void RotateFile(string path, string newPath);
}