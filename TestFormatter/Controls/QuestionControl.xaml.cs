using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using TestFormatter.Models;
using Microsoft.Win32;
using System.Windows.Media.Imaging;
using System.ComponentModel;

namespace TestFormatter.Controls
{
    public partial class QuestionControl : UserControl, INotifyPropertyChanged
    {
        private Question question;
        public Question Question
        {
            get { return question; }
            set
            {
                question = value;
                DataContext = question; // Set the DataContext to enable binding
                UpdateHeaderText(); // Update header text initially
            }
        }

        //Header text property
        public string HeaderText { get; private set; } // Property for the header

        //Even handler so FormatterPage can modify Test class properly when big changes are done to the question (type change / deletion)
        public event EventHandler<Question> QuestionTypeChanged;
        public event EventHandler<Question> QuestionDeleted;

        public QuestionControl()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void UpdateHeaderText()
        {
            // Set HeaderText based on Question properties (e.g., number and type)
            HeaderText = $"Question #{question.Number}: {question.Type}";
            OnPropertyChanged(nameof(HeaderText)); // Notify the UI of the update
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

        //TextBox logic so it adjust veritcal size based on user input
        private void QuestionTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                // Measure the height required to display all the text
                textBox.Measure(new Size(textBox.Width, double.PositiveInfinity));
                double requiredHeight = textBox.ExtentHeight;

                // Check if the content overflows or wraps onto a new line
                if (requiredHeight > textBox.ActualHeight)
                {
                    // Add padding only if a new line is needed
                    textBox.Height = requiredHeight + 5;
                }
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
                UpdateHeaderText(); // Refresh header after type changes
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

                AddMatchingButton.Children.Clear();
                if (selectedType == "Matching")
                {
                    Button addMatchingPairs = new Button
                    {
                        Content = " Add Matching pair ",
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Margin = new Thickness(5, 0, 5, 5)
                    };
                    addMatchingPairs.Click += AddMatchingPairs_Click;
                    AddMatchingButton.Children.Add(addMatchingPairs);
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

        private void AddMatchingPairs_Click(object sender, RoutedEventArgs e)
        {
            if (question.Type == "Matching")
            {
                StackPanel matchingPanel = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Margin = new Thickness(100, 5, 5, 5)
                };

                TextBox matchingTextBox1 = new TextBox
                {
                    Width = 200,
                    Margin = new Thickness(5)
                };

                TextBox matchingTextBox2 = new TextBox
                {
                    Width = 200,
                    Margin = new Thickness(5)
                };

                matchingTextBox1.LostFocus += (s, args) =>
                {
                    int index = AdditionalOptionsPanel.Children.IndexOf(matchingPanel);
                    if (index >= 0 && index < question.Matching.Count)
                    {
                        question.Matching[index] = new Tuple<string, string>(matchingTextBox1.Text, question.Matching[index].Item2);
                    }
                    else if (index >= question.Matching.Count)
                    {
                        question.Matching.Add(new Tuple<string, string>(matchingTextBox1.Text, matchingTextBox2.Text));
                    }
                };

                matchingTextBox2.LostFocus += (s, args) =>
                {
                    int index = AdditionalOptionsPanel.Children.IndexOf(matchingPanel);
                    if (index >= 0 && index < question.Matching.Count)
                    {
                        question.Matching[index] = new Tuple<string, string>(question.Matching[index].Item1, matchingTextBox2.Text);
                    }
                    else if (index >= question.Matching.Count)
                    {
                        question.Matching.Add(new Tuple<string, string>(matchingTextBox1.Text, matchingTextBox2.Text));
                    }
                };

                Button deleteButton = new Button
                {
                    Content = "❌",
                    Margin = new Thickness(5)
                };
                deleteButton.Click += (s, args) =>
                {
                    int positionOfOption = AdditionalOptionsPanel.Children.IndexOf(matchingPanel) - 1;

                    if (positionOfOption >= 0 && positionOfOption < question.Matching.Count)
                    {
                        AdditionalOptionsPanel.Children.Remove(matchingPanel);
                        question.Matching.RemoveAt(positionOfOption);
                    }
                };

                matchingPanel.Children.Add(matchingTextBox1);
                matchingPanel.Children.Add(matchingTextBox2);
                matchingPanel.Children.Add(deleteButton);

                AdditionalOptionsPanel.Children.Add(matchingPanel);
                matchingTextBox1.Focus();
                matchingTextBox2.Focus();
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
                    int index = AdditionalOptionsPanel.Children.IndexOf(optionPanel);
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
        private void AddImageCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            AddPicture.Visibility = Visibility.Visible;
        }

        private void AddPicture_Click(object sender, RoutedEventArgs e)
        {
            // Open file dialog to select an image
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                BitmapImage image = new BitmapImage(new Uri(openFileDialog.FileName));
                question.QuestionImage = image; // Store the image in the Question class

                QuestionImageControl.Source = image; // Display the image in the UI
                QuestionImageControl.Visibility = Visibility.Visible;
            }
        }
        private void AddImageCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            // Remove the image if the checkbox is unchecked
            question.QuestionImage = null;
            QuestionImageControl.Source = null;
            QuestionImageControl.Visibility = Visibility.Collapsed;
            AddPicture.Visibility = Visibility.Collapsed;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
