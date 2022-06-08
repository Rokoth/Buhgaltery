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
    /// Логика взаимодействия для FormulaEditWindow.xaml
    /// </summary>
    public partial class FormulaEditWindow : Window
    {
        public event EventHandler<FormulaAddArgs> OnUpdateEvent;
        public FormulaEditWindow()
        {
            InitializeComponent();
        }

        internal void ShowDialog(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
