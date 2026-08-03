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
            //var db = new DatabaseConnection();
            
            desktop.MainWindow = new MainWindow
            {
                //Change back if needed. Used to pass db contxt to main app
                DataContext = new MainWindowViewModel(),
                //DataContext = new MainWindowViewModel(db),
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}