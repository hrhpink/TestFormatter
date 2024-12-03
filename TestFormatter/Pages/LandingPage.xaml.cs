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
using System.Text.Json;
using TestFormatter.Models;
using Microsoft.Win32;
using TestFormatter.Controls;

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
            this.NavigationService.Navigate(new FormatterPage(new Exam()));
        }

        private void LoadExam_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog to select a JSON file
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "JSON Files (*.json)|*.json|All Files (*.*)|*.*",
                Title = "Select a Test File to Load"
            };

            // Show the dialog and get the selected file
            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                try
                {
                    // Read the JSON file
                    string jsonContent = File.ReadAllText(filePath);
                    // Deserialize JSON into ExamWrapper object
                    ExamJsonWrapper examJson = JsonSerializer.Deserialize<ExamJsonWrapper>(jsonContent);
                    
                    if (examJson != null)
                    {
                        Exam loadedExam = new Exam(examJson.Questions)
                        {
                            IncludeNameField = examJson.IncludeNameField,
                            IncludeIDField = examJson.IncludeIDField,
                            IncludeDateField = examJson.IncludeDateField,
                            IncludeClassField = examJson.IncludeClassField,
                            IncludeSectionField = examJson.IncludeSectionField,
                            IncludeGradeField = examJson.IncludeGradeField,
                            QuestionLimit = examJson.QuestionLimit,
                            NumberOfPoints = examJson.NumberOfPoints
                        };

                        // Navigate to formatter
                        this.NavigationService.Navigate(new FormatterPage(loadedExam, true));
                    }
                    else
                    {
                        MessageBox.Show("Failed to load test. The file might be empty or invalid.", "Load Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    // Handle any errors that occur during the process
                    MessageBox.Show($"An error occurred while loading the file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
