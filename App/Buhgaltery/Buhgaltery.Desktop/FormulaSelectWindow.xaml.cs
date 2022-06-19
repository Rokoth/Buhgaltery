﻿using System;
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
    /// Логика взаимодействия для FormulaSelectWindow.xaml
    /// </summary>
    public partial class FormulaSelectWindow : Window
    {
        public FormulaSelectWindow()
        {
            InitializeComponent();
        }

        public event EventHandler<ElementSelectedArgs> OnElementSelected;

    }

    public class ElementSelectedArgs
    {
        public Guid Id { get; set; }
        public string Name { get; internal set; }
    }
}
