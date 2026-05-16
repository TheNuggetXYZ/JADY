using System.Threading.Tasks;
using Avalonia.Controls.ApplicationLifetimes;

namespace JADY.Services;

public interface IAppStartupService
{
    Task AppStartup(IClassicDesktopStyleApplicationLifetime desktop);
}