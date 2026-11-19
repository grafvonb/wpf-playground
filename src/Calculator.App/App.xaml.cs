using System.Windows;
using Calculator.App.ViewModels;
using Calculator.Service;

namespace Calculator.App
{
    public partial class App : Application
    {
        private void OnStartup(object sender, StartupEventArgs e)
        {
            var window = new MainWindow
            {
                DataContext = new MainViewModel(new CalculatorFacade())
            };
            window.Show();
        }
    }
}
