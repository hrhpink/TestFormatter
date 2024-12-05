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
using System.IO;
using System.ComponentModel;
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using System.Diagnostics;

namespace TestFormatter.Pages
{
    /// <summary>
    /// Interaction logic for FormatterPage.xaml
    /// </summary>
    public partial class FormatterPage : Page
    {
        //Initialization of Exam class to hold questions
        private Exam currentExam;

        public FormatterPage(Exam exam = null, bool newExam = false) 
        {
            currentExam = exam;
        
            InitializeComponent();
            //Set DataContext to bind the XAML to the currentExam object 
            this.DataContext = currentExam; 

            if(newExam == true)
            {
                manual_load_questions();
            }
        }

        public void manual_load_questions()
        {
            foreach (Question question in currentExam.Questions)
            {
                var questionControl = new QuestionControl();

                // Initialize the control with the question data
                questionControl.Initialize(question);

                // Subscribe to the QuestionTypeChanged event
                questionControl.QuestionTypeChanged += QuestionControl_QuestionTypeChanged;

                // Subscribe to the QuestionDeleted event
                questionControl.QuestionDeleted += QuestionControl_QuestionDeleted;

                // Add the new QuestionControl to the Question Panel
                QuestionsPanel.Children.Insert(QuestionsPanel.Children.Count - 1, questionControl);
            }
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

            // Create a new Free Response Question by default
            var newQuestion = new Question
            {
                Type = "Free Response", // Default type
                QuestionText = "",
                Points = 1,
                NumLines = 1 // Default number of lines for Free Response
            };
            currentExam.AddQuestion(newQuestion);

            // Create a new QuestionControl and initialize it
            var questionControl = new QuestionControl();
            questionControl.Initialize(newQuestion);

            // Subscribe to the QuestionTypeChanged and QuestionDeleted events
            questionControl.QuestionTypeChanged += QuestionControl_QuestionTypeChanged;
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
            currentExam.DeleteQuestion(deletedQuestion);
        }

        //Go back to landing page
        private void GoBack_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.GoBack();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
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
                Filter = "Json File (*.json)|*.json",
                Title = "Save Exam Questions"
            };
            if (saveFileDialog.ShowDialog() == true)
            {
                // Assuming 'exam' is an instance of your Exam class
                currentExam.ExportToJsonFile(currentExam, saveFileDialog.FileName);
                MessageBox.Show("Questions saved successfully.", "Save", MessageBoxButton.OK, MessageBoxImage.Information);
            }
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
            
            bool hasImages = currentExam.Questions.Any(q => q.QuestionImage != null);

            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = hasImages ? "PDF files (*.pdf)|*.pdf" : "Text files (*.txt)|*.txt"
            };

            // Show the SaveFileDialog and check if the user selected a file
            if (saveFileDialog.ShowDialog() == true)
            {
                // Get the file path selected by the user
                string filePath = saveFileDialog.FileName;

                // Assuming 'exam' is the instance of the Exam class in the current context
                currentExam.ExportToPdf(saveFileDialog.FileName);
                MessageBox.Show("Questions exported successfully.", "Export", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        
        // INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
