using SQLitePCL;
using System.Configuration;
using System.Data;
using System.Windows;

namespace SpiderGame
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            Batteries.Init(); // Инициализация SQLitePCL
        }
    }

}
