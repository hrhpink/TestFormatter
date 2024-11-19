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

        //Even handler so FormatterPage can modify Test class properly when big changes are done to the question (type change / deletion / Num change)
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
        private void TextBoxAdjustment_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                //wrapAroundHeight = amount of vertical space the characters are currently using
                //textboxHeight = current height of textbox

                // Measure the height required to display all the text
                textBox.Measure(new Size(textBox.Width, double.PositiveInfinity));
                double wrapAroundHeight = Math.Round((textBox.ExtentHeight),0);
                double textboxHeight = textBox.Height;

                // Check if the content overflows or wraps onto a new line
                if (wrapAroundHeight > textboxHeight)
                {
                    // Add padding only if a new line is needed
                    textBox.Height = textboxHeight + 20;
                }
                else if ((wrapAroundHeight + 5 < textboxHeight) && (textboxHeight-wrapAroundHeight > 18))
                {
                  textBox.Height = textboxHeight - 20;

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

                 ////////////////////
                //Mulltiple Choice//
               ////////////////////
                AddOptionButton.Children.Clear();
                if (selectedType == "Multiple Choice")
                {
                    //Modify QuestionTextBlock to say enter question:
                    QuestionTextBlock.FontSize = 12;
                    QuestionTextBlock.Text = "Enter Question:";

                    Button addOptionButton = new Button
                    {
                        Content = " Add Option ",
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Margin = new Thickness(5, 0, 5, 0)
                    };
                    addOptionButton.Click += AddOptionButton_Click;
                    AddOptionButton.Children.Add(addOptionButton);
                }
                 ////////////
                //Matching//
               ////////////
                if (selectedType == "Matching")
                {
                    //Modify QuestionTextBlock to say enter instructions:
                    QuestionTextBlock.FontSize = 12;
                    QuestionTextBlock.Text = "Enter Instructions:";

                    StackPanel matchingPanel = new StackPanel()
                    {
                        Orientation = Orientation.Horizontal,
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Center,
                        Margin = new Thickness(5, 0, 5, 0)
                    };

                    //Instructions
                    TextBlock matchingInstructionsTextBlock = new TextBlock
                    {
                        FontSize = 12,
                        Text = "Matching Instructions:\n• Input questions and matching options in the corresponding boxes below.\n• Each question and option must be separated by a new line (Enter).",
                        VerticalAlignment = VerticalAlignment.Center,
                        Margin = new Thickness(5, 10, 0, 10) // Margin to add spacing below label
                    };

                    // Left vertical panel for questions/words
                    StackPanel leftPanel = new StackPanel()
                    {
                        Orientation = Orientation.Vertical,
                        Margin = new Thickness(0, 0, 10, 0)
                    };

                    // Label for questions/words
                    TextBlock matchingTextBlock1 = new TextBlock
                    {
                        FontSize = 12,
                        Text = "Questions",
                        VerticalAlignment = VerticalAlignment.Center,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Margin = new Thickness(5, 0, 0, 5) // Margin to add spacing below label
                    };

                    // TextBox for questions/words
                    TextBox matchingTextBox1 = new TextBox
                    {
                        Width = 222.5,
                        Height = 150,
                        Margin = new Thickness(0, 5, 5, 5),
                        AcceptsReturn = true,
                        TextWrapping = TextWrapping.Wrap
                    };

                    // Right vertical panel for matching options
                    StackPanel rightPanel = new StackPanel()
                    {
                        Orientation = Orientation.Vertical,
                        Margin = new Thickness(10, 0, 5, 0)
                    };

                    // Label for options to match
                    TextBlock matchingTextBlock2 = new TextBlock
                    {
                        FontSize = 12,
                        Text = "Matching Options",
                        VerticalAlignment = VerticalAlignment.Center,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Margin = new Thickness(5, 0, 0, 5) // Margin to add spacing below label
                    };

                    // TextBox for options to match
                    TextBox matchingTextBox2 = new TextBox
                    {
                        Width = 222.5,
                        Height = 150,
                        Margin = new Thickness(0, 5, 5, 5),
                        AcceptsReturn = true,
                        TextWrapping = TextWrapping.Wrap
                    };

                    // Update Matching[0] with new lines in matchingTextBox1
                    matchingTextBox1.LostFocus += (s, args) =>
                    {
                        question.Matching.Item1.Clear(); // Clear existing entries
                        question.Matching.Item1.AddRange(matchingTextBox1.Text.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries));
                    };

                    // Update Matching[1] with new lines in matchingTextBox2
                    matchingTextBox2.LostFocus += (s, args) =>
                    {
                        question.Matching.Item2.Clear(); // Clear existing entries
                        question.Matching.Item2.AddRange(matchingTextBox2.Text.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries));
                    };


                    //Add Children to panel so they are displayed
                    AdditionalOptionsPanel.Children.Add(matchingInstructionsTextBlock);
                    AdditionalOptionsPanel.Children.Add(matchingPanel);

                    // Add the TextBlocks and TextBoxes to their respective vertical panels
                    leftPanel.Children.Add(matchingTextBlock1);
                    leftPanel.Children.Add(matchingTextBox1);
                    rightPanel.Children.Add(matchingTextBlock2);
                    rightPanel.Children.Add(matchingTextBox2);

                    // Add the vertical panels to the main horizontal panel
                    matchingPanel.Children.Add(leftPanel);
                    matchingPanel.Children.Add(rightPanel);
                }
                 //////////////////
                //Free Response //
               //////////////////
                if (selectedType == "Free Response")
                {
                    //Modify QuestionTextBlock to say enter question:
                    QuestionTextBlock.FontSize = 12;
                    QuestionTextBlock.Text = "Enter Question:";

                    StackPanel linePanel = new StackPanel()
                    {
                        Orientation = Orientation.Horizontal,
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Center,
                        Margin = new Thickness(5, 0, 5, 0)
                    };
                    TextBlock lineTextBlock = new TextBlock
                    {
                        FontSize = 12,
                        Text = "Input number of lines for answer space:",
                        VerticalAlignment= VerticalAlignment.Center,
                        //Margin= new Thickness(5, 0, 0, 0)

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

                 //////////////
                //True/False//
               //////////////
                if (selectedType == "True/False")
                {
                    //Modify QuestionTextBlock to say enter instructions:
                    QuestionTextBlock.FontSize = 12;
                    QuestionTextBlock.Text = "Enter Instructions:";

                    // Label for True/False question box
                    TextBlock TrueOrFalseTextBlock = new TextBlock
                    {
                        FontSize = 12,
                        Text = "True or False Instructions:\n• Enter questions in the textbox below.\n• All questions must be separated by a new line (Enter).\nNote: Test Formatter will include the True/False options in the final output.",
                        VerticalAlignment = VerticalAlignment.Center,
                        Margin = new Thickness(5, 10, 0, 5) // Margin to add spacing below label
                    };

                    // TextBox for True/False quesitons
                    TextBox TrueOrFalseTextBox = new TextBox
                    {
                        HorizontalAlignment = HorizontalAlignment.Left,
                        Width = 470,
                        Height = 150,
                        Margin = new Thickness(5),
                        AcceptsReturn = true,
                        TextWrapping = TextWrapping.Wrap
                    };

                    // Update Matching[0] with new lines in matchingTextBox1
                    TrueOrFalseTextBox.LostFocus += (s, args) =>
                    {
                        question.TrueOrFalse.Clear(); // Clear existing entries
                        question.TrueOrFalse.AddRange(TrueOrFalseTextBox.Text.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries));
                    };

                    AdditionalOptionsPanel.Children.Add(TrueOrFalseTextBlock);
                    AdditionalOptionsPanel.Children.Add(TrueOrFalseTextBox);
                }

                  /////////////////////
                 //Fill in the Blank//
                ////////////////////
                if (selectedType == "Fill in the Blank")
                {
                    //Adjusting question box message
                    QuestionTextBlock.FontSize = 12;
                    QuestionTextBlock.Text = "Enter Question:";

                    // Label for Fill in the blank question box
                    TextBlock FITBTextBlock = new TextBlock
                    {
                        FontSize= 12,
                        Text = "For Fill in the Blank, input your question in the box above.\nNote: Make sure to include a sequence of underscores (_) for the missing term that\nis sufficiently long for the correct answer.",
                        VerticalAlignment = VerticalAlignment.Center,
                        Margin = new Thickness(5, 10, 0, 5) // Margin to add spacing below label
                    };

                    //Add Children to panel so they are displayed
                    AdditionalOptionsPanel.Children.Add(FITBTextBlock);

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
                    Margin = new Thickness(106, 0, 5, 5)
                };

                TextBox optionTextBox = new TextBox
                {
                    Height = 25,
                    Width = 300,
                    Margin = new Thickness(5),
                    AcceptsReturn = true, // Allows multiline input
                    TextWrapping = TextWrapping.Wrap
                };

                //TextChanged event for dynamic height adjustment
                optionTextBox.TextChanged += TextBoxAdjustment_TextChanged;

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
                    int positionOfOption = AdditionalOptionsPanel.Children.IndexOf(optionPanel);

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

        private void UpArrowButton_Click(object sender, RoutedEventArgs e)
        {
        }
        private void DownArrowButton_Click(object sender, RoutedEventArgs e)
        {
        }
    }
}
