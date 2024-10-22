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

namespace TestFormatter.Windows
{
    /// <summary>
    /// Interaction logic for FileNameInput.xaml
    /// </summary>
    public partial class FileNameInput : Window
    {
        public FileNameInput()
        {
            InitializeComponent();
            DataContext = this;
        }

        private string _fileName;
        public string fileName
        {
            get { return _fileName; }
            set { 
                _fileName = value;
            }
            //TODO: store file name somewhere.
        }

        private void ConfirmName_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
