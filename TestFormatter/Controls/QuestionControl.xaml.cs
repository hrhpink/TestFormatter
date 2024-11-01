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
                if (question is MultipleChoiceQuestion mcq)
                {
                    LoadMultipleChoiceOptions(mcq);
                }
            }
        }
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

        private void QuestionTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (QuestionTypeComboBox.SelectedItem is ComboBoxItem selectedType)
            {
                string questionType = selectedType.Content.ToString();

                // Clear any existing controls in the AdditionalOptionsPanel
                AdditionalOptionsPanel.Children.Clear();

                if (questionType == "Multiple Choice" && question is MultipleChoiceQuestion mcq)
                {
                    LoadMultipleChoiceOptions(mcq);

                    // Create a button to add options
                    Button addOptionButton = new Button
                    {
                        Content = "Add Option",
                        Margin = new Thickness(0, 5, 0, 5)
                    };
                    addOptionButton.Click += (s, args) => AddOptionTextBox(mcq, mcq.Options.Count);
                    AdditionalOptionsPanel.Children.Add(addOptionButton);
                }
            }
        }
        private void LoadMultipleChoiceOptions(MultipleChoiceQuestion mcq)
        {
            AdditionalOptionsPanel.Children.Clear();

            for (int i = 0; i < mcq.Options.Count; i++)
            {
                AddOptionTextBox(mcq, i);
            }

            // Add button to add new options
            Button addOptionButton = new Button
            {
                Content = "Add Option",
                Margin = new Thickness(0, 5, 0, 5)
            };
            addOptionButton.Click += (s, e) => AddOptionTextBox(mcq, mcq.Options.Count);
            AdditionalOptionsPanel.Children.Add(addOptionButton);
        }

        private void AddOptionTextBox(MultipleChoiceQuestion mcq, int index)
        {
            // Ensure there's a list entry for this option
            if (index >= mcq.Options.Count)
            {
                mcq.Options.Add(string.Empty);  // Initialize new option in list
            }

            // Create StackPanel to hold TextBox and delete button
            StackPanel optionPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(0, 5, 0, 5)
            };

            // Create TextBox for the option
            TextBox optionTextBox = new TextBox
            {
                Width = 200,
                Margin = new Thickness(0, 5, 0, 5),
                TextWrapping = TextWrapping.Wrap,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                Text = mcq.Options[index]
            };

            // Update the list entry when the TextBox loses focus
            optionTextBox.LostFocus += (s, e) =>
            {
                mcq.Options[index] = optionTextBox.Text;
            };

            // Delete button to remove the option
            Button deleteOptionButton = new Button
            {
                Content = "❌",
                Width = 30,
                Margin = new Thickness(5, 0, 0, 0)
            };
            deleteOptionButton.Click += (s, e) =>
            {
                mcq.Options.RemoveAt(index);
                AdditionalOptionsPanel.Children.Remove(optionPanel);
                ReloadOptions(mcq);
            };

            // Add TextBox and delete button to the panel
            optionPanel.Children.Add(optionTextBox);
            optionPanel.Children.Add(deleteOptionButton);

            // Add panel to AdditionalOptionsPanel
            AdditionalOptionsPanel.Children.Insert(AdditionalOptionsPanel.Children.Count - 1, optionPanel); // Insert before add button
        }

        private void ReloadOptions(MultipleChoiceQuestion mcq)
        {
            // Clear and reload all options in case of deletions
            LoadMultipleChoiceOptions(mcq);
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
