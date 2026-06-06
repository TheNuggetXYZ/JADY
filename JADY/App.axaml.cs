using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Metadata;
using JADY.Factories;
using JADY.Services;
using JADY.UI.Views.Windows;
using JADY.UI.Views.Dialogs;
using JADY.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

[assembly: XmlnsDefinition("https://github.com/avaloniaui", "JADY.UI.Views.Controls")]

namespace JADY;

public partial class App : Application
{
    public static ServiceProvider? ServiceProvider { get; private set; }
    
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
        
#if DEBUG
        this.AttachDeveloperTools(); 
#endif
    }

    public override async void OnFrameworkInitializationCompleted()
    {
        ConfigureServices(out var serviceProvider);
        
        ServiceProvider = serviceProvider;

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            await serviceProvider.GetRequiredService<IAppStartupService>().AppStartup(desktop);

        base.OnFrameworkInitializationCompleted();
    }

    private static void ConfigureServices(out ServiceProvider serviceProvider)
    {
        var collection = new ServiceCollection();

        collection.AddLogging(builder =>
        {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Information);
        });
        
        // Services
        collection.AddSingleton<IEncryptionService, EncryptionService>();
        collection.AddSingleton<IAppVisualService, AppVisualService>();
        collection.AddSingleton<ISaveFsService, SaveFsService>();
        collection.AddSingleton<ISaveIoService, SaveIoService>();
        collection.AddSingleton<ISaveService, SaveService>();
        collection.AddSingleton<IAppStartupService, AppStartupService>();
        collection.AddSingleton<IWindowService, WindowService>();
        collection.AddSingleton<IDiaryService, DiaryService>();
        
        // Factories
        collection.AddSingleton<IDiaryEntryViewModelFactory, DiaryEntryViewModelFactory>();
        collection.AddSingleton<IDiaryViewModelFactory, DiaryViewModelFactory>();
        
        // View Models
        collection.AddSingleton<MainWindowViewModel>();
        
        // Views
        collection.AddTransient<SettingsWindow>();
        collection.AddTransient<AddEntryWindow>();
        collection.AddTransient<AddDiaryWindow>();
        collection.AddTransient<EditDiaryWindow>();
        collection.AddTransient<EditEntryWindow>();
        collection.AddTransient<EndEntryWindow>();
        collection.AddTransient<UnsavedChangesWindow>();
        collection.AddTransient<WelcomeWindow>();
        collection.AddTransient<LoginWindow>();
        collection.AddTransient<LinkEntryWindow>();

        serviceProvider = collection.BuildServiceProvider();
    }
}