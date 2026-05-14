using Avalonia.Controls.ApplicationLifetimes;

namespace JADY.Services;

public interface IAppStartupService
{
    void AppStartup(IClassicDesktopStyleApplicationLifetime desktop);
}