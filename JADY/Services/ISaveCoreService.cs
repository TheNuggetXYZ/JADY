using System.Threading.Tasks;
using JADY.Models;

namespace JADY.Services;

public interface ISaveCoreService
{
    void Write(string filePath, JadySave save);
    JadySave Read(string filePath);
}