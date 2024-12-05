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
using TestFormatter.Pages;

namespace TestFormatter.Windows
{
    /// <summary>
    /// Interaction logic for FileNameInput.xaml
    /// </summary>
    /// 



    public partial class FileNameInput : Window
    {
        public FileNameInput()
        {
            InitializeComponent();
            DataContext = this;
        }

        private string fileNameInputText = "";
        public string FileNameInputText
        {
            get { return fileNameInputText; }
            set { 
                fileNameInputText = value;
            }
        }


        private void ConfirmName_Click(object sender, RoutedEventArgs e)
        {
            TestFormatter.Pages.LandingPage.FileName = FileNameInputText;
            this.Close();
        }
    }
}
