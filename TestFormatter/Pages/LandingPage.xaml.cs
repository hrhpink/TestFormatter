using System;
using System.Collections.Generic;
using System.IO;
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

namespace TestFormatter.Pages
{
    /// <summary>
    /// Interaction logic for LandingPage.xaml
    /// </summary>
    public partial class LandingPage : Page
    {
        public LandingPage()
        {
            InitializeComponent();
        }

        private void CreateExam_Click(object sender, RoutedEventArgs e)
        {

            //Ask user to enter file name
            Window win2 = new Windows.FileNameInput();
            win2.ShowDialog();
       
            //Navigate to formatter
            this.NavigationService.Navigate(new Uri("/Pages/FormatterPage.xaml", UriKind.Relative));
        }

        private void LoadExam_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.DefaultExt = ".csv"; // Default extension
            dialog.Filter = "Text documents (.csv)|*.csv"; // Filter files by extension

            // Show open file dialog box
           if (dialog.ShowDialog() == true)
            {
                string filepath = dialog.FileName;
                //TODO: store name in a place that program can access at runtime so that it can display it
                //OR do this in FileNameInput.xaml.cs
                var fileStream = dialog.OpenFile();
                using (StreamReader sr = new StreamReader(fileStream)) { 
                //TODO: parse csv file
                }

            }
        }
    }
}
