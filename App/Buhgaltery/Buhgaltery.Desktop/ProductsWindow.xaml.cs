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
    /// Логика взаимодействия для ProductsWindow.xaml
    /// </summary>
    public partial class ProductsWindow : Window
    {
        private IServiceProvider _serviceProvider;
        private readonly IDataService<Product, ProductFilter, ProductUpdater> _dataService;
        private readonly ILogger logger;

        private bool isLoaded = false;

        private int page = 0;
        private int allPages = 1;
        private bool needRefresh = false;

        public ProductsWindow(IServiceProvider serviceProvider)
        {
            InitializeComponent();
            _serviceProvider = serviceProvider;

            _dataService = _serviceProvider.GetRequiredService<IDataService<Product, ProductFilter, ProductUpdater>>();

            logger = _serviceProvider.GetRequiredService<ILogger<ProductsWindow>>();

            Task.Factory.StartNew(RunTimer, TaskCreationOptions.LongRunning);
            DataGridMain.MouseDoubleClick += EditButton_Click;

            isLoaded = true;
        }

        private async Task RunTimer()
        {
            while (true)
            {
                if (needRefresh)
                {
                    Dispatcher.Invoke(() => FillTable());
                    needRefresh = false;
                }
                await Task.Delay(1000);
            }
        }

        private async void FillTable()
        {
            try
            {
                var perPage = int.Parse(CountTextBox.Text);
                DataGridMain.Items.Clear();
                //TODO: filters (userId, from, to)
                var result = await _dataService.Get(new ProductFilter(perPage, page, null, null, null, false, null, null));
                allPages = (result.Count % perPage == 0) ? (result.Count / perPage) : ((result.Count / perPage) + 1);
                foreach (var item in result)
                {
                    DataGridMain.Items.Add(item);
                }

                CountLabel.Content = $"Страница {page + 1} из {allPages}";
            }
            catch (Exception ex)
            {
                if (ex is AggregateException aex)
                {
                    var message = "";
                    var stack = "";
                    foreach (var exs in aex.InnerExceptions)
                    {
                        message += exs.Message + "\r\n";
                        stack += exs.StackTrace + "\r\n";
                    }
                    logger.LogError($"Ошибка при получении списка продуктов: {message} {stack}");
                    MessageBox.Show($"Ошибка при получении списка продуктов: {message}");
                }
                else
                {
                    logger.LogError($"Ошибка при получении списка продуктов: {ex.Message} {ex.StackTrace}");
                    MessageBox.Show($"Ошибка при получении списка продуктов: {ex.Message}");
                }
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            AddProduct();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            EditSelected();
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Удалить продукт?", "Удаление продукт", MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.OK)
            {
                var row = DataGridMain.SelectedItem;
                if (row != null)
                {
                    if (await _dataService.Delete(((Product)row).Id))
                    {
                        FillTable();
                    }
                }
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void FilterButton_Click(object sender, RoutedEventArgs e)
        {
            FillTable();
        }

        private void DataGridMain_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void DataGridCell_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void DataGridCell_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            EditSelected();
        }

        private void BeginButton_Click(object sender, RoutedEventArgs e)
        {
            IncPage(false, true);
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            IncPage(false, false);
        }

        private void ForwardButton_Click(object sender, RoutedEventArgs e)
        {
            IncPage(true, false);
        }

        private void EndButton_Click(object sender, RoutedEventArgs e)
        {
            IncPage(true, true);
        }

        private void CountTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void DataGridMain_Loaded(object sender, RoutedEventArgs e)
        {
            FillTable();
        }

        private void EditSelected()
        {
            var addTreeWindow = _serviceProvider.GetRequiredService<ProductEditWindow>();
            var row = DataGridMain.SelectedItem;
            if (row != null)
            {
                addTreeWindow.ShowDialog(((Product)row).Id);
            }

            FillTable();
        }

        private void AddProduct()
        {
            var addTreeWindow = _serviceProvider.GetRequiredService<ProductAddWindow>();
            addTreeWindow.ShowDialog();

            FillTable();
        }

        private void IncPage(bool inc, bool end)
        {
            bool changed = false;
            if (inc)
            {
                if (page + 1 < allPages)
                {
                    if (end)
                    {
                        page = allPages - 1;
                    }
                    else
                    {
                        page++;
                    }
                    changed = true;
                }
            }
            else
            {
                if (page > 0)
                {
                    if (end)
                    {
                        page = 0;
                    }
                    else
                    {
                        page--;
                    }
                    changed = true;
                }
            }
            if (changed)
            {
                FillTable();
            }
        }

        private void CountTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {

        }
    }
}
