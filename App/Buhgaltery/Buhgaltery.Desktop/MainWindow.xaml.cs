using Buhgaltery.Desktop.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Buhgaltery.Desktop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private IServiceProvider _serviceProvider;
        private IAuthService _authService;        

        public MainWindow(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _authService = _serviceProvider.GetRequiredService<IAuthService>();

            InitializeComponent();

            Task.Factory.StartNew(RunTimer, TaskCreationOptions.LongRunning);
        }

        private async Task RunTimer()
        {
            while (true)
            {
                Dispatcher.Invoke(() => Refresh());
                await Task.Delay(5000);
            }
        }

        private void ProductsButton_Click(object sender, RoutedEventArgs e)
        {
            ShowDialogWindow<ProductsWindow>();
        }                

        private void AddReserve_Click(object sender, RoutedEventArgs e)
        {
            ShowDialogWindow<AddReserveWindow>();            
        }

        private void AddIncoming_Click(object sender, RoutedEventArgs e)
        {
            ShowDialogWindow<AddIncomingWindow>();            
        }

        private void AddOutgoing_Click(object sender, RoutedEventArgs e)
        {
            ShowDialogWindow<AddOutgoingWindow>();
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            RequireAuth();
            Refresh();
        }

        private void FormulasButton_Click(object sender, RoutedEventArgs e)
        {
            ShowDialogWindow<FormulasWindow>();
        }

        private void IncomingsButton_Click(object sender, RoutedEventArgs e)
        {
            ShowDialogWindow<IncomingsWindow>();
        }

        private void OutgoingsButton_Click(object sender, RoutedEventArgs e)
        {
            ShowDialogWindow<OutgoingsWindow>();
        }

        private void ReservesButton_Click(object sender, RoutedEventArgs e)
        {
            ShowDialogWindow<ReservesWindow>();
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            ShowDialogWindow<SettingsWindow>();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            ShowDialogWindow<AboutWindow>();
        }

        private void Issue_Click(object sender, RoutedEventArgs e)
        {
            ShowDialogWindow<IssueWindow>();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            RequireAuth();
            Refresh();
        }

        private void Refresh()
        {
            throw new NotImplementedException();
        }

        private void RequireAuth()
        {
            if (!_authService.IsAuth)
            {
                var connectWindow = _serviceProvider.GetRequiredService<ServerConnectWindow>();
                connectWindow.ShowDialog();
                if (!_authService.IsAuth)
                {
                    Close();
                }
            }
        }

        private void ShowDialogWindow<T>() where T : Window
        {
            RequireAuth();
            var window = _serviceProvider.GetRequiredService<T>();
            window.ShowDialog();
            Refresh();
        }
    }
}
