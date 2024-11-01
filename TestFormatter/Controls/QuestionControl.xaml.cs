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
            }
        }
        public QuestionControl()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void QuestionTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (QuestionTypeComboBox.SelectedItem is ComboBoxItem selectedType)
            {
                string questionType = selectedType.Content.ToString();

                // Clear any existing controls in the AdditionalOptionsPanel
                AdditionalOptionsPanel.Children.Clear();

                if (questionType == "Multiple Choice")
                {
                    // Create a button to add options
                    Button addOptionButton = new Button
                    {
                        Content = "Add Option",
                        Margin = new Thickness(0, 5, 0, 5)
                    };
                    addOptionButton.Click += AddOptionButton_Click;
                    AdditionalOptionsPanel.Children.Add(addOptionButton);
                }
            }
        }

        private void AddOptionButton_Click(object sender, RoutedEventArgs e)
        {
            // Add a new TextBox for each option in multiple choice
            TextBox optionTextBox = new TextBox
            {
                Width = 200,
                Margin = new Thickness(0, 5, 0, 5),
                TextWrapping = TextWrapping.Wrap,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto
            };

            // Add a delete button next to each option
            Button deleteOptionButton = new Button
            {
                Content = "Delete",
                Width = 50,
                Margin = new Thickness(5, 0, 0, 0)
            };
            deleteOptionButton.Click += (s, args) =>
            {
                AdditionalOptionsPanel.Children.Remove(optionTextBox);
                AdditionalOptionsPanel.Children.Remove(deleteOptionButton);
            };

            AdditionalOptionsPanel.Children.Add(optionTextBox);
            AdditionalOptionsPanel.Children.Add(deleteOptionButton);
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
