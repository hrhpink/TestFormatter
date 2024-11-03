using System;
using System.Windows;
using System.Windows.Controls;
using TestFormatter.Models;

namespace TestFormatter.Controls
{
    /// <summary>
    /// Interaction logic for QuestionControl.xaml
    /// </summary>
    public partial class QuestionControl : UserControl
    {
        private Question question;
        public Question Question
        {
            get { return question; }
            set
            {
                question = value;
                DataContext = question; // Set the DataContext to enable binding
            }
        }

        public event EventHandler<Question> QuestionTypeChanged;

        public QuestionControl()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            // This will trigger binding updates when the user clicks out of the TextBox
            if (sender is TextBox textBox)
            {
                textBox.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            // Placeholder for delete logic
        }

        private void QuestionTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (QuestionTypeComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                string selectedType = selectedItem.Content.ToString();
                Question newQuestion = null;

                // Update the question type dynamically
                if (selectedType == "Multiple Choice")
                {
                    if (!(question is MultipleChoiceQuestion))
                    {
                        newQuestion = new MultipleChoiceQuestion
                        {
                            QuestionText = question.QuestionText,
                            Points = question.Points,
                            Number = question.Number,
                            NumLines = question.NumLines
                        };
                    }
                }
                else if (selectedType == "Free Response")
                {
                    if (!(question is FreeResponseQuestion))
                    {
                        newQuestion = new FreeResponseQuestion
                        {
                            QuestionText = question.QuestionText,
                            Points = question.Points,
                            Number = question.Number,
                            NumLines = question.NumLines
                        };
                    }
                }

                if (newQuestion != null)
                {
                    question = newQuestion;
                    DataContext = question;
                    QuestionTypeChanged?.Invoke(this, question);
                }

                // Update the AdditionalOptionsPanel based on the selected type
                AdditionalOptionsPanel.Children.Clear();
                if (selectedType == "Multiple Choice")
                {
                    Button addOptionButton = new Button
                    {
                        Content = "Add Option",
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Margin = new Thickness(5)
                    };
                    addOptionButton.Click += AddOptionButton_Click;
                    AdditionalOptionsPanel.Children.Add(addOptionButton);
                }
            }
        }

        private void AddOptionButton_Click(object sender, RoutedEventArgs e)
        {
            if (question is MultipleChoiceQuestion mcQuestion)
            {
                // Create a StackPanel to hold the new option TextBox and delete button
                StackPanel optionPanel = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Margin = new Thickness(5)
                };

                // Create the TextBox for user input
                TextBox optionTextBox = new TextBox
                {
                    Width = 300,
                    Margin = new Thickness(5)
                };
                optionTextBox.LostFocus += (s, args) =>
                {
                    int index = AdditionalOptionsPanel.Children.IndexOf(optionPanel) - 1; // Subtract 1 to account for the Add Option button
                    if (index >= 0 && index < mcQuestion.Options.Count)
                    {
                        mcQuestion.Options[index] = optionTextBox.Text;
                    }
                    else if (index >= mcQuestion.Options.Count)
                    {
                        mcQuestion.Options.Add(optionTextBox.Text);
                    }
                };

                // Create the delete button
                Button deleteButton = new Button
                {
                    Content = "Delete",
                    Margin = new Thickness(5)
                };
                deleteButton.Click += (s, args) =>
                {
                    // Get the index of the optionPanel directly from the mcQuestion.Options
                    int positionOfOption = AdditionalOptionsPanel.Children.IndexOf(optionPanel) - 1; // Adjust for Add Option button

                    if (positionOfOption >= 0 && positionOfOption < mcQuestion.Options.Count)
                    {
                        AdditionalOptionsPanel.Children.Remove(optionPanel);
                        mcQuestion.Options.RemoveAt(positionOfOption);

                    }
                };

                // Add the TextBox and delete button to the StackPanel
                optionPanel.Children.Add(optionTextBox);
                optionPanel.Children.Add(deleteButton);

                // Add the option panel to the AdditionalOptionsPanel
                AdditionalOptionsPanel.Children.Add(optionPanel);

                // Focus the TextBox so the user can start typing immediately
                optionTextBox.Focus();
            }
        }
    }
}
