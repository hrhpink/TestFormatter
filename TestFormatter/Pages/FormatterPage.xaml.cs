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
using TestFormatter.Models;

namespace TestFormatter.Pages
{
    /// <summary>
    /// Interaction logic for FormatterPage.xaml
    /// </summary>
    public partial class FormatterPage : Page
    {
        //Initialization of Exam class to hold questions
        private Exam _exam = new Exam();
        private int _questionCounter = 1; // Track question numbers

        public FormatterPage()
        {
            InitializeComponent();
        }

        //Add Questions code
        private void AddQuestionButton_Click(object sender, RoutedEventArgs e)
        {
            // Create the main panel for each question
            var questionPanel = new StackPanel
            {
                Margin = new Thickness(10),
                Background = Brushes.White,
            };

            var grid = new Grid();

            // Define rows and columns
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            // Question Type ComboBox
            var typeLabel = new Label { Content = "Question Type:", VerticalAlignment = VerticalAlignment.Center };
            Grid.SetRow(typeLabel, 0);
            Grid.SetColumn(typeLabel, 0);
            grid.Children.Add(typeLabel);

            var typeComboBox = new ComboBox { Width = 150, Margin = new Thickness(5) };
            typeComboBox.ItemsSource = new List<string> { "Short Answer", "Long Answer", "Multiple Choice" };
            Grid.SetRow(typeComboBox, 0);
            Grid.SetColumn(typeComboBox, 1);
            grid.Children.Add(typeComboBox);

            // Question Number TextBox
            var questionNumberLabel = new Label { Content = "Question Number:", VerticalAlignment = VerticalAlignment.Center };
            Grid.SetRow(questionNumberLabel, 0);
            Grid.SetColumn(questionNumberLabel, 2);
            grid.Children.Add(questionNumberLabel);

            var questionNumberTextBox = new TextBox { Width = 50, Margin = new Thickness(5) };
            Grid.SetRow(questionNumberTextBox, 0);
            Grid.SetColumn(questionNumberTextBox, 3);
            grid.Children.Add(questionNumberTextBox);

            // Points TextBox
            var pointsLabel = new Label { Content = "Points:", VerticalAlignment = VerticalAlignment.Center };
            Grid.SetRow(pointsLabel, 1);
            Grid.SetColumn(pointsLabel, 0);
            grid.Children.Add(pointsLabel);

            var pointsTextBox = new TextBox { Width = 50, Margin = new Thickness(5) };
            Grid.SetRow(pointsTextBox, 1);
            Grid.SetColumn(pointsTextBox, 1);
            grid.Children.Add(pointsTextBox);

            // Question Text
            var questionTextLabel = new Label { Content = "Question Text:", VerticalAlignment = VerticalAlignment.Center };
            Grid.SetRow(questionTextLabel, 2);
            Grid.SetColumn(questionTextLabel, 0);
            grid.Children.Add(questionTextLabel);

            var questionTextBox = new TextBox { Margin = new Thickness(5), Height = 30, Width = 300 };
            Grid.SetRow(questionTextBox, 2);
            Grid.SetColumn(questionTextBox, 1);
            Grid.SetColumnSpan(questionTextBox, 3);
            grid.Children.Add(questionTextBox);

            // Answer TextBox (for long answer or multiple choice options)
            var answerTextBox = new TextBox
            {
                Margin = new Thickness(5),
                Height = 100,
                Text = "Space for answer",
                Foreground = Brushes.Red,
                HorizontalAlignment = HorizontalAlignment.Stretch
            };
            Grid.SetRow(answerTextBox, 3);
            Grid.SetColumn(answerTextBox, 0);
            Grid.SetColumnSpan(answerTextBox, 4);
            grid.Children.Add(answerTextBox);

            // Add button to manage options for Multiple Choice (Initially hidden)
            var optionsPanel = new StackPanel { Visibility = Visibility.Collapsed, Margin = new Thickness(5) };
            var addOptionButton = new Button { Content = "Add Option", Width = 100, Margin = new Thickness(0, 5, 0, 5) };
            addOptionButton.Click += (s, ev) => optionsPanel.Children.Add(new TextBox { Width = 300, Margin = new Thickness(0, 5, 0, 5) });
            optionsPanel.Children.Add(addOptionButton);
            grid.Children.Add(optionsPanel);
            Grid.SetRow(optionsPanel, 3);
            Grid.SetColumn(optionsPanel, 0);
            Grid.SetColumnSpan(optionsPanel, 4);

            // Show/Hide Options Panel based on Type Selection
            typeComboBox.SelectionChanged += (s, ev) =>
            {
                answerTextBox.Visibility = (typeComboBox.SelectedItem.ToString() == "Multiple Choice") ? Visibility.Collapsed : Visibility.Visible;
                optionsPanel.Visibility = (typeComboBox.SelectedItem.ToString() == "Multiple Choice") ? Visibility.Visible : Visibility.Collapsed;
            };

            // Save and Delete Buttons
            var saveButton = new Button { Content = "Save", Width = 75, Margin = new Thickness(5) };
            var deleteButton = new Button { Content = "Delete", Width = 75, Margin = new Thickness(5) };

            saveButton.Click += (s, ev) =>
            {
                var questionType = typeComboBox.SelectedItem?.ToString();
                Question question;

                if (questionType == null)
                {
                    MessageBox.Show("Please select a question type.");
                    return;
                }

                switch (questionType)
                {
                    case "Short Answer":
                        question = new ShortAnswerQuestion { Text = questionTextBox.Text, Points = int.Parse(pointsTextBox.Text) };
                        break;
                    case "Long Answer":
                        question = new LongAnswerQuestion { Text = questionTextBox.Text, Points = int.Parse(pointsTextBox.Text) };
                        break;
                    case "Multiple Choice":
                        var mcQuestion = new MultipleChoiceQuestion { Text = questionTextBox.Text, Points = int.Parse(pointsTextBox.Text) };
                        foreach (TextBox option in optionsPanel.Children.OfType<TextBox>())
                            mcQuestion.Options.Add(option.Text);
                        question = mcQuestion;
                        break;
                    default:
                        return;
                }

                _exam.AddQuestion(question);
                MessageBox.Show("Question added!");
            };

            deleteButton.Click += (s, ev) =>
            {
                QuestionsPanel.Children.Remove(questionPanel);
                MessageBox.Show("Question deleted!");
            };

            // Add buttons to bottom row
            var buttonPanel = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right };
            buttonPanel.Children.Add(saveButton);
            buttonPanel.Children.Add(deleteButton);

            Grid.SetRow(buttonPanel, 4);
            Grid.SetColumn(buttonPanel, 3);
            grid.Children.Add(buttonPanel);

            // Add grid to main question panel and then to QuestionsPanel
            questionPanel.Children.Add(grid);
            QuestionsPanel.Children.Add(questionPanel);
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
