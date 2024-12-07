//Interaction logic for file name input box
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
    public partial class FileNameInput : Window
    {
        public FileNameInput()
        {
            InitializeComponent();
            DataContext = this;
        }

        private string fileNameInputText = "";

        //Sets user input to variable
        public string FileNameInputText
        {
            get { return fileNameInputText; }
            set { 
                fileNameInputText = value;
            }
        }

        private void ConfirmName_Click(object sender, RoutedEventArgs e)
        {
            //Stores file name input for later use when exporting exam
            TestFormatter.Pages.LandingPage.FileName = FileNameInputText;
            this.Close();
        }
    }
}
