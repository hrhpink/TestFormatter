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
        }

        //Add Questions code
        private void AddQuestionButton_Click(object sender, RoutedEventArgs e)
        {
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

        }
    }
}
