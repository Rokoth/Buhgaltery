using Buhgaltery.Contract.Model;
using Buhgaltery.Desktop.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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
using System.Windows.Shapes;

namespace Buhgaltery.Desktop
{
    /// <summary>
    /// Логика взаимодействия для FormulaWindow.xaml
    /// </summary>
    public partial class FormulaWindow : Window
    {
        private readonly IServiceProvider _serviceProvider;        
        public event EventHandler<FormulaAddArgs> OnUpdateEvent;
        private readonly ILogger<FormulaWindow> _logger;
      
        private Guid _id;

        public FormulaWindow(IServiceProvider serviceProvider)
        {
            InitializeComponent();
            _serviceProvider = serviceProvider;
            _logger = serviceProvider.GetRequiredService<ILogger<FormulaWindow>>();
        }

        public async void ShowDialog(Guid id)
        {
            try
            {               
                _id = id;
                var dataService = _serviceProvider.GetRequiredService<IDataService<Formula>>();
                var formula = await dataService.Get(id);
                if (formula != null)
                {
                    NameTextBox.Text = formula.Name;
                    DefaultCheckBox.IsChecked = formula.IsDefault;
                    FormulaTextBox.Text = formula.Text;
                    ShowDialog();
                }
                else
                {
                    MessageBox.Show($"Формула не найдена");
                    Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при открытии окна редактирования формулы: {ex.Message}");
                _logger.LogError($"Ошибка при открытии окна редактирования формулы: {ex.Message} {ex.StackTrace}");
                Close();
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            var editWindow = _serviceProvider.GetRequiredService<FormulaEditWindow>();
            editWindow.OnUpdateEvent += EditWindow_OnUpdateEvent;
            editWindow.ShowDialog(_id);            
        }

        private void EditWindow_OnUpdateEvent(object sender, FormulaAddArgs e)
        {
            OnUpdateEvent?.Invoke(this, new FormulaAddArgs()
            {
                Id = e.Id
            });
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }

    public class FormulaAddArgs
    {
        public Guid Id { get; set; }
    }
}
