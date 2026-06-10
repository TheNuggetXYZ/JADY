using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;

namespace JADY.Services;

public class ShutdownService : IShutdownService
{
    public void Shutdown()
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            desktop.Shutdown();
    }
}