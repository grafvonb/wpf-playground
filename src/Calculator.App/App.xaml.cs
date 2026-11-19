using System;
using System.Configuration;
using System.Windows;
using Calculator.App.ViewModels;
using Calculator.Service;

namespace Calculator.App
{
    public partial class App : Application
    {
        private void OnStartup(object sender, StartupEventArgs e)
        {
            ICalculatorFacade facade;
            try
            {
                var connection = ConfigurationManager.ConnectionStrings["CalculatorDb"];
                facade = CalculatorFacadeFactory.Create(
                    ConfigurationManager.AppSettings["CalculatorBackend"],
                    connection?.ConnectionString);
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message, "Configuration error", MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown(1);
                return;
            }

            var window = new MainWindow
            {
                DataContext = new MainViewModel(facade)
            };
            window.Show();
        }
    }
}
