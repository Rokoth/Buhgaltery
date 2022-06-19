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
    /// Логика взаимодействия для SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        private Guid _id;
        private Guid _formulaId;
        
        private IServiceProvider _serviceProvider;
        private ILogger<SettingsWindow> _logger;       
        private IDataService<User, UserFilter, UserUpdater> _dataService;
        
        public SettingsWindow(IServiceProvider serviceProvider)
        {
            InitializeComponent();
            _serviceProvider = serviceProvider;
            _dataService = _serviceProvider.GetRequiredService<IDataService<User, UserFilter, UserUpdater>>();
            _logger = _serviceProvider.GetRequiredService<ILogger<SettingsWindow>>();
        }

        public async void ShowDialogWindow(Guid id)
        {
            _id = id;
            var settings = await _dataService.GetItem(_id);
            NameTextBox.Text = settings.Name;
            DescriptionTextBox.Text = settings.Description;
            AddPeriodTextBox.Text = settings.AddPeriod.ToString();
            EmailTextBox.Text = settings.Email;
            FormulaTextBox.Text = settings.Formula;
            LoginTextBox.Text = settings.Login;
            PasswordTextBox.Password = "******";
            LeafOnlyCheckBox.IsChecked = settings.LeafOnly;
            _formulaId = settings.FormulaId;
            ShowDialog();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            UserUpdater settings = new UserUpdater() { 
              FormulaId = _formulaId,
              LeafOnly = LeafOnlyCheckBox.IsChecked ?? false,
              
              AddPeriod = int.Parse(AddPeriodTextBox.Text),
              Description = DescriptionTextBox.Text,
              Email = EmailTextBox.Text,
              Id = _id,
              Login = LoginTextBox.Text,
              Name = NameTextBox.Text,
              PasswordChanged = PasswordTextBox.Password != "******",
              Password = PasswordTextBox.Password,
              DefaultReserveValue = decimal.Parse(DefaultReserveValueTextBox.Text)              
            };
            await _dataService.Update(settings);
        }

        private void SelectFormulaButton_Click(object sender, RoutedEventArgs e)
        {
            var formulaWindow = _serviceProvider.GetRequiredService<FormulaSelectWindow>();
            formulaWindow.OnElementSelected += FormulaWindow_OnFormulaSelected;
            formulaWindow.ShowDialog();
        }

        private void FormulaWindow_OnFormulaSelected(object sender, ElementSelectedArgs e)
        {
            SetFormula(e.Id, e.Name);
        }

        private void SetFormula(Guid id, string name)
        {
            _formulaId = id;            
            FormulaTextBox.Text = name;
        }
    }

    
}
