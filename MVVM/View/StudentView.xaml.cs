using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
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
using oop11.MVVM.Model;
using oop4_5.Core;

namespace oop11.MVVM.View
{
    /// <summary>
    /// Логика взаимодействия для StudentView.xaml
    /// </summary>
    public partial class StudentView : UserControl
    {
        private static StudentView studentView;

        public StudentView()
        {
            InitializeComponent();
            studentView = this;
        }
    }
}
