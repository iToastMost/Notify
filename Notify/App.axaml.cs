using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using Avalonia.Markup.Xaml;
using Notify.Data;
using Notify.ViewModels;
using Notify.Views;

namespace Notify;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var db = new DatabaseConnection();
            
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainWindowViewModel(db),
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}