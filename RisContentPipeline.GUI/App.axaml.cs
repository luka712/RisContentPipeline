using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RisContentPipeline.GUI.Services;
using RisContentPipeline.GUI.ViewModels;
using MainWindow = RisContentPipeline.GUI.Windows.MainWindow;

namespace RisContentPipeline.GUI;

/// <summary>
/// The application class.
/// </summary>
public partial class App : Application
{
    /// <summary>
    /// The application services.
    /// </summary>
    public static IServiceProvider Services { get; private set; } = null!;
    
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var services = new ServiceCollection();
        ConfigureServices(services);
        Services = services.BuildServiceProvider();
        
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var mainViewModel = new MainViewModel();
            AttachMainViewModel(Services, mainViewModel);
            var mainWindow = new MainWindow()
            {
                DataContext = mainViewModel
            };
            desktop.MainWindow = mainWindow;
        }
        
        base.OnFrameworkInitializationCompleted();
    }
    
    private static void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<IPipelineSystem, PipelineSystem>();
        services.AddSingleton<AppConfig>();
        services.AddSingleton<WindowsService>();
        services.AddSingleton<AssetsService>();
        services.AddSingleton<ViewerService>();
        services.AddSingleton<LocalWebServer>();
        services.AddSingleton<UserPreferencesService>();
        services.AddSingleton<MessageService>();
        services.AddSingleton<FoldersService>();
    }
    
    private static void AttachMainViewModel(IServiceProvider container, MainViewModel viewModel)
    {
        container.GetRequiredService<UserPreferencesService>().ViewModel = viewModel;
        container.GetRequiredService<LocalWebServer>().ViewModel = viewModel;
    }
}