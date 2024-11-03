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
            var newQuestion = new FreeResponseQuestion();
            currentExam.AddQuestion(newQuestion);

            // Create a new instance of QuestionControl and set its Question property
            var questionControl = new QuestionControl
            {
                Question = newQuestion
            };

            // Subscribe to the QuestionTypeChanged event
            questionControl.QuestionTypeChanged += QuestionControl_QuestionTypeChanged;

            // Add the new QuestionControl to the Question Panel
            QuestionsPanel.Children.Add(questionControl);
        }

        //Event to handle when question type changes
        private void QuestionControl_QuestionTypeChanged(object sender, Question newQuestion)
        {
            // Update the existing question in the Exam class with the new question type
            int index = currentExam.Questions.IndexOf(((QuestionControl)sender).Question);
            if (index != -1)
            {
                currentExam.Questions[index] = newQuestion;
            }
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
