//Code behind for the main exam formatting page. Includes:
//Functions for updating UI when exam is imported
//Updating question numbers on the back end
//Adding new question and checking if it exceeds question limit
//Question type changed
//Question deleted
//Go back to previous page
//Save exam
//Export exam
//Swap question order with arrows
//Shuffle order of questions
//Updating UI when above changes occur
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
using TestFormatter.Windows;
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using System.Diagnostics;

namespace TestFormatter.Pages
{
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
            
            currentExam.FileName = LandingPage.FileName;


            if(newExam == true)
            {
                manual_load_questions();
            }
        }

        //Manually load in questions when exam is imported
        public void manual_load_questions()
        {
            foreach (Question question in currentExam.Questions)
            {
                var questionControl = new QuestionControl
                {
                    Question = question,
                    ParentFormatterPage = this
                };

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

        //Update question numbers when order changed
        private void UpdateQuestionNumbers()
        {
            for (int i = 0; i < currentExam.Questions.Count; i++)
            {
                currentExam.Questions[i].Number = i + 1; // Update the question's number
            }

            foreach (var control in QuestionsPanel.Children.OfType<QuestionControl>())
            {
                control.UpdateHeaderText(); // Assuming this updates the displayed number
            }
        }

        //Add Question to exam
        private void AddQuestionButton_Click(object sender, RoutedEventArgs e)
        {
            //If newly added question exceeds count for max number of questions specified in options, do not add question
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
                QuestionText = ""
            };
            currentExam.AddQuestion(newQuestion);

            // Create a new QuestionControl and initialize it
            var questionControl = new QuestionControl
            {
                Question = newQuestion,
                ParentFormatterPage = this
            };

            // Subscribe to the QuestionTypeChanged and QuestionDeleted events
            questionControl.QuestionTypeChanged += QuestionControl_QuestionTypeChanged;
            questionControl.QuestionDeleted += QuestionControl_QuestionDeleted;

            // Add the new QuestionControl to the Question Panel
            QuestionsPanel.Children.Insert(QuestionsPanel.Children.Count - 1, questionControl);

            // Update question numbers
            UpdateQuestionNumbers();
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

            // Update the UI to reflect the new order
            UpdateQuestionNumbers();
        }

        //Go back to landing page
        private void GoBack_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.GoBack();
        }

        //Save exam into .json for later editing
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
                Title = "Save Exam Questions",
                FileName = currentExam.FileName
            };
            if (saveFileDialog.ShowDialog() == true)
            {
                // Assuming 'exam' is an instance of your Exam class
                currentExam.ExportToJsonFile(currentExam, saveFileDialog.FileName);
                MessageBox.Show("Questions saved successfully.", "Save", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        //Export exam to .pdf or .txt
        private void Export_Click(object sender, RoutedEventArgs e)
        {
            string validationMessage;

            //If validation fails, display error message
            if (!currentExam.ValidateQuestions(out validationMessage))
            {
                MessageBox.Show(validationMessage, "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            
            bool hasImages = currentExam.Questions.Any(q => q.QuestionImage != null);

            //opens file dialog for exporting exam
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {

                Title = "Export Exam Questions",
                FileName = currentExam.FileName,
                Filter = hasImages ? "PDF files (*.pdf)|*.pdf" : "Text files (*.txt)|*.txt"

            };

            // Show the SaveFileDialog and check if the user selected a file
            if (saveFileDialog.ShowDialog() == true)
            {
                // Get the file path selected by the user
                string filePath = saveFileDialog.FileName;

                // Assuming 'exam' is the instance of the Exam class in the current context
                if(hasImages == true)
                {
                    currentExam.ExportToPdf(saveFileDialog.FileName);
                }
                else
                {
                    currentExam.ExportToTextFile(saveFileDialog.FileName);
                }
                    
                MessageBox.Show("Questions exported successfully.", "Export", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        
        // INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        //Swaps question order in code behind when UpArrow or DownArrow is clicked
        public void swap_questions(int currentIndex, bool up_arrow)
        {
            if (up_arrow)
            {
                Question tempQuestion = currentExam.Questions[currentIndex];
                currentExam.Questions[currentIndex] = currentExam.Questions[currentIndex - 1];
                currentExam.Questions[currentIndex - 1] = tempQuestion;
            }
            else
            {
                Question tempQuestion = currentExam.Questions[currentIndex + 1];
                currentExam.Questions[currentIndex + 1] = currentExam.Questions[currentIndex];
                currentExam.Questions[currentIndex] = tempQuestion;
            }
        }

        //Shuffles question order in code behind
        private void ShuffleQuestionsButton_Click(object sender, RoutedEventArgs e)
        {
            var random = new Random();

            // Shuffle the questions in the currentExam
            var shuffledQuestions = currentExam.Questions.OrderBy(q => random.Next()).ToList();

            // Update the question numbers in the shuffled list
            for (int i = 0; i < shuffledQuestions.Count; i++)
            {
                shuffledQuestions[i].Number = i + 1; // Assuming question numbers start at 1
            }

            // Update the existing collection instead of reassigning
            currentExam.Questions.Clear();
            foreach (var question in shuffledQuestions)
            {
                currentExam.Questions.Add(question);
            }

            // Update the UI to reflect the new order
            UpdateQuestionNumbers();

            // Rearrange the QuestionControl objects in the QuestionsPanel
            UpdateQuestionControlOrder();
        }

        //Updates question order in UI
        private void UpdateQuestionControlOrder()
        {
            // Locate the Add Question button and temporarily store it
            Button addQuestionButton = null;
            foreach (var child in QuestionsPanel.Children)
            {
                if (child is Button button && button.Name == "AddQuestionButton")
                {
                    addQuestionButton = button;
                    break;
                }
            }

            // Remove the Add Question button if it exists
            if (addQuestionButton != null)
            {
                QuestionsPanel.Children.Remove(addQuestionButton);
            }

            // Create a dictionary to map questions to their controls
            var questionControlMap = QuestionsPanel.Children.OfType<QuestionControl>()
                .ToDictionary(qc => qc.Question);

            // Clear the QuestionsPanel but retain the existing QuestionControl objects
            QuestionsPanel.Children.Clear();

            // Add controls back in the new order
            foreach (var question in currentExam.Questions)
            {
                if (questionControlMap.TryGetValue(question, out var questionControl))
                {
                    QuestionsPanel.Children.Add(questionControl);

                    // Update the UI to reflect the new number
                    questionControl.UpdateHeaderText();
                }
            }

            // Re-add the Add Question button as the last element
            if (addQuestionButton != null)
            {
                QuestionsPanel.Children.Add(addQuestionButton);
            }

            // Update both the question numbers and their headers
            UpdateQuestionNumbers();
        }
    }
}
