using System.IO;

namespace JADY.Services;

public interface ISaveFsService
{
    bool TryRotateFile(string source, string destination, bool overwrite);
}