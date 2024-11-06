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
using TestFormatter.Controls;
using TestFormatter.Models;
using Microsoft.Win32;

namespace TestFormatter.Pages
{
    /// <summary>
    /// Interaction logic for FormatterPage.xaml
    /// </summary>
    public partial class FormatterPage : Page
    {
        //Initialization of Exam class to hold questions
        private Exam currentExam = new Exam();

        public FormatterPage()
        {
            InitializeComponent();

            //Set DataContext to bind the XAML to the currentExam object 
            DataContext = currentExam; 
        }

        //Add Questions code
        private void AddQuestionButton_Click(object sender, RoutedEventArgs e)
        {

            if (currentExam.QuestionLimit > 0 && currentExam.Questions.Count >= currentExam.QuestionLimit)
            {
                MessageBox.Show($"You have reached the maximum number of questions ({currentExam.QuestionLimit}).",
                                "Question Limit Reached",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                return; // Exit without adding a new question
            }

            // Create a new FreeResponseQuestion by default
            var newQuestion = new Question();
            currentExam.AddQuestion(newQuestion);

            // Create a new instance of QuestionControl and set its Question property
            var questionControl = new QuestionControl
            {
                Question = newQuestion
            };

            // Subscribe to the QuestionTypeChanged event
            questionControl.QuestionTypeChanged += QuestionControl_QuestionTypeChanged;

            // Subscribe to the QuestionDeleted event
            questionControl.QuestionDeleted += QuestionControl_QuestionDeleted;

            // Add the new QuestionControl to the Question Panel
            QuestionsPanel.Children.Insert(QuestionsPanel.Children.Count - 1, questionControl);
        }

        //Event to handle when question type changes
        private void QuestionControl_QuestionTypeChanged(object sender, Question updatedQuestion)
        {
            // Find the index of the original question and replace it with the updated question
            int index = currentExam.Questions.IndexOf(((QuestionControl)sender).Question);
            if (index != -1)
            {
                currentExam.Questions[index] = updatedQuestion;
            }
        }

        //Event to handle when question is deleted
        private void QuestionControl_QuestionDeleted(object sender, Question deletedQuestion)
        {
            // Remove the question from currentExam
            currentExam.Questions.Remove(deletedQuestion);
        }

        //Go back to landing page
        private void GoBack_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.GoBack();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Export_Click(object sender, RoutedEventArgs e)
        {
            string validationMessage;

            // Assuming 'exam' is an instance of your Exam class
            if (!currentExam.ValidateQuestions(out validationMessage))
            {
                MessageBox.Show(validationMessage, "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Text File (*.txt)|*.txt",
                Title = "Save Exam Questions"
            };
            if (saveFileDialog.ShowDialog() == true)
            {
                // Assuming 'exam' is an instance of your Exam class
                currentExam.ExportToTextFile(saveFileDialog.FileName);
                MessageBox.Show("Questions exported successfully.", "Export", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        private void Checkbox_Changed(object sender, RoutedEventArgs e)
        {
            // Display current values of each property
            MessageBox.Show($"Include Name: {currentExam.IncludeNameField}\n");
        }
    }
}
