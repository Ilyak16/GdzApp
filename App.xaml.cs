using System.Windows;

namespace GdzApp
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            Data.Database.Initialize();
        }
    }
}