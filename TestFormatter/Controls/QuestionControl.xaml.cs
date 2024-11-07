using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using TestFormatter.Models;

namespace TestFormatter.Controls
{
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

        //Even handler so FormatterPage can modify Test class properly when big changes are done to the question (type change / deletion)
        public event EventHandler<Question> QuestionTypeChanged;
        public event EventHandler<Question> QuestionDeleted;

        public QuestionControl()
        {
            InitializeComponent();
            DataContext = this;
        }
        
        //TextBox logic so it saves content when user clicks away
        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                textBox.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();
                QuestionTypeChanged?.Invoke(this, question);
            }
        }

        //Question delete button logic
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            // Show a confirmation message box
            MessageBoxResult result = MessageBox.Show(
                "Are you sure you want to delete this question?",
                "Confirm Delete",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            // Check if the user confirmed the deletion
            if (result == MessageBoxResult.Yes)
            {
                var parent = this.Parent as Panel;
                if (parent != null)
                {
                    parent.Children.Remove(this);
                }
                QuestionDeleted?.Invoke(this, question);
            }
        }

        //Logic for ComboBox to determine what type of question it is and display different information
        private void QuestionTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (QuestionTypeComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                string selectedType = selectedItem.Content.ToString();

                // Set the question type and adjust properties
                question.SetType(selectedType);
                DataContext = question;
                QuestionTypeChanged?.Invoke(this, question);

                // Update the AdditionalOptionsPanel based on the selected type
                AdditionalOptionsPanel.Children.Clear();
                AddOptionButton.Children.Clear();
                if (selectedType == "Multiple Choice")
                {
                    Button addOptionButton = new Button
                    {
                        Content = " Add Option ",
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Margin = new Thickness(5, 0, 5, 5)
                    };
                    addOptionButton.Click += AddOptionButton_Click;
                    AddOptionButton.Children.Add(addOptionButton);
                }

                if (selectedType == "Free Response")
                {
                    StackPanel linePanel = new StackPanel()
                    {
                        Orientation = Orientation.Horizontal,
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Center,
                        Margin = new Thickness(5, 0, 5, 0)
                    };
                    TextBlock lineTextBlock = new TextBlock
                    {
                        Text = "Input number of lines for answer space:",
                        VerticalAlignment= VerticalAlignment.Center,
                        Margin= new Thickness(5, 0, 0, 0)

                    };
                    TextBox lineTextBox = new TextBox
                    {
                        Width = 40,
                        Height = 25,
                        Margin = new Thickness(5),
                        VerticalContentAlignment = VerticalAlignment.Center
                    };


                    // Set up data binding for NumLines
                    Binding numLinesBinding = new Binding("NumLines")
                    {
                        Source = question, // Make sure 'question' is the DataContext object containing NumLines
                        UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                    };
                    lineTextBox.SetBinding(TextBox.TextProperty, numLinesBinding);

                    // Attach the LostFocus event handler
                    lineTextBox.LostFocus += TextBox_LostFocus;

                    //Add Children to panel so they are displayed
                    AdditionalOptionsPanel.Children.Add(linePanel);
                    linePanel.Children.Add(lineTextBlock);
                    linePanel.Children.Add(lineTextBox);
                }
            }
        }

        //Logic for AddOption button for multiple choice questions
        private void AddOptionButton_Click(object sender, RoutedEventArgs e)
        {
            if (question.Type == "Multiple Choice")
            {
                StackPanel optionPanel = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Margin = new Thickness(145, 5, 5, 5)
                };

                TextBox optionTextBox = new TextBox
                {
                    Width = 300,
                    Margin = new Thickness(5)
                };
                optionTextBox.LostFocus += (s, args) =>
                {
                    int index = AdditionalOptionsPanel.Children.IndexOf(optionPanel) - 1;
                    if (index >= 0 && index < question.Options.Count)
                    {
                        question.Options[index] = optionTextBox.Text;
                    }
                    else if (index >= question.Options.Count)
                    {
                        question.Options.Add(optionTextBox.Text);
                    }
                };

                Button deleteButton = new Button
                {
                    Content = "❌",
                    Margin = new Thickness(5)
                };
                deleteButton.Click += (s, args) =>
                {
                    int positionOfOption = AdditionalOptionsPanel.Children.IndexOf(optionPanel) - 1;

                    if (positionOfOption >= 0 && positionOfOption < question.Options.Count)
                    {
                        AdditionalOptionsPanel.Children.Remove(optionPanel);
                        question.Options.RemoveAt(positionOfOption);
                    }
                };

                optionPanel.Children.Add(optionTextBox);
                optionPanel.Children.Add(deleteButton);

                AdditionalOptionsPanel.Children.Add(optionPanel);
                optionTextBox.Focus();
            }
        }
    }
}
