using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json;
using System.ComponentModel;
using System.Printing.IndexedProperties;
using PdfSharp.Pdf;
using PdfSharp.Drawing;

namespace TestFormatter.Models
{
    public class Exam : INotifyPropertyChanged
    {
        public List<Question> Questions { get; private set; } = new List<Question>();

        public bool IncludeNameField { get; set; }
        public bool IncludeIDField { get; set; }
        public bool IncludeDateField { get; set; }
        public bool IncludeClassField { get; set; }
        public bool IncludeSectionField { get; set; }
        public bool IncludeGradeField { get; set; }

        public int QuestionLimit { get; set; } = 0; //Default limit (0 means no limit)
        public double NumberOfPoints { get; set; }

        public int QuestionCount => Questions.Count; // Property to track number of questions

        public Exam(List<Question>? questions = null)
        {
            if (questions != null)
            {
                Questions = questions;
            }
        }
        public void AddQuestion(Question question)
        {
            Questions.Add(question);
            OnPropertyChanged(nameof(QuestionCount));
        }

        public void DeleteQuestion(Question question)
        {
            Questions.Remove(question);
            OnPropertyChanged(nameof(QuestionCount));
        }

        public bool ValidateQuestions(out string validationMessage)
        {
            double sumOfPoints = 0;
            for (int i = 0; i < Questions.Count; i++)
            {
                Question question = Questions[i];
                sumOfPoints += question.Points;
                if (question.QuestionText == null)
                {
                    validationMessage = $"One or more of your questions are missing the question text.";
                    return false;
                }
                else if (question.Points <= 0)
                {
                    validationMessage = $"Make sure all questions have points assigned.";
                    return false;
                }
                else if (question.Type == "Multiple Choice")
                {
                    // Check if options exist for multiple choice questions
                    if (question.Options == null || question.Options.Count == 0)
                    {
                        validationMessage = $"Question {i+1} is multiple choice but has no options.";
                        return false;
                    }
                    else
                    {
                        foreach (var option in question.Options)
                        {
                            if(option == "")
                            {
                                validationMessage = $"Question {i+1} is multiple choice but has empty options.";
                                return false;
                            }
                        }
                    }
                }
                else if (question.Type == "Free Response")
                {
                    // Check if numLines is set for free response questions
                    if (question.NumLines <= 0)
                    {
                        validationMessage = $"Question {i+1} is free response but has no placeholder lines specified.";
                        return false;
                    }
                }
            }
            if (NumberOfPoints != sumOfPoints)
            {
                validationMessage = $"The \"Total Points\" field does not match the sum of the question point values.";
                return false;
            }

            validationMessage = ""; // No issues
            return true;
        }

        public void ExportToPdf(string filePath)
        {
            // Create a new PDF document
            PdfDocument document = new PdfDocument();
            document.Info.Title = "Exam Questions";

            // Add a page to the document
            PdfPage page = document.AddPage();
            XGraphics gfx = XGraphics.FromPdfPage(page);
            XFont font = new XFont("Verdana", 12);
            XFont boldFont = new XFont("Verdana", 12);

            double yPosition = 20; // Starting Y position for the first line

            if (IncludeNameField)
            {
                gfx.DrawString("Name: ________________________", font, XBrushes.Black, new XRect(20, yPosition, page.Width, 0));
                yPosition += 20;
            }
            if (IncludeDateField)
            {
                gfx.DrawString("Date: __/__/____", font, XBrushes.Black, new XRect(20, yPosition, page.Width, 0));
                yPosition += 20;
            }

            // Include Class field if enabled
            if (IncludeClassField)
            {
                gfx.DrawString("Class: ___________", font, XBrushes.Black, new XRect(20, yPosition, page.Width, 0));
                yPosition += 20;
            }

            // Include Section field if enabled
            if (IncludeSectionField)
            {
                gfx.DrawString("Section: ___________", font, XBrushes.Black, new XRect(20, yPosition, page.Width, 0));
                yPosition += 20;
            }

            // Include Grade field if enabled
            if (IncludeGradeField)
            {
                if (NumberOfPoints > 0)
                {
                    gfx.DrawString($"Grade: ___/{NumberOfPoints}", font, XBrushes.Black, new XRect(20, yPosition, page.Width, 0));
                }
                else
                {
                    gfx.DrawString("Grade: ___/___", font, XBrushes.Black, new XRect(20, yPosition, page.Width, 0));
                }
                yPosition += 20;
            }

            foreach (var question in Questions)
            {
                // Add question number and points
                gfx.DrawString($"Question {Questions.IndexOf(question) + 1}", boldFont, XBrushes.Black, new XRect(20, yPosition, page.Width, 0));
                yPosition += 20;

                gfx.DrawString($"({question.Points} points) {question.QuestionText}", font, XBrushes.Black, new XRect(20, yPosition, page.Width, 0));
                yPosition += 20;
                
                if (question.QuestionImage != null)
                {
                    // Load and get the image
                    XImage image = XImage.FromFile(question.QuestionImage.UriSource.LocalPath);

                    // Define the maximum size for the image
                    double maxWidth = page.Width - 40; // Subtracting margins
                    double maxHeight = page.Height - 100; // Adjust as needed

                    // Calculate the scale factor to fit the image within the bounds
                    double scaleFactor = Math.Min(maxWidth / image.PixelWidth, maxHeight / image.PixelHeight);

                    // Calculate the scaled width and height
                    double scaledWidth = image.PixelWidth * scaleFactor;
                    double scaledHeight = image.PixelHeight * scaleFactor;

                    // Draw the scaled image
                    gfx.DrawImage(image, 20, yPosition, scaledWidth, scaledHeight);

                    // Update the yPosition after the image
                    yPosition += scaledHeight + 20; // Adjust for the next content
                }
                // If the question is Multiple Choice, display the options
                if (question.Type == "Multiple Choice")
                {
                    for (int i = 0; i < question.Options.Count; i++)
                    {
                        gfx.DrawString($"{i + 1}. {question.Options[i]}", font, XBrushes.Black, new XRect(40, yPosition, page.Width, 0));
                        yPosition += 20;
                    }
                }
                else
                {
                    // Otherwise, display blank lines as placeholders
                    for (int i = 0; i < question.NumLines; i++)
                    {
                        gfx.DrawString(new string('_', 80), font, XBrushes.Black, new XRect(20, yPosition, page.Width, 0)); // Placeholder line
                        yPosition += 20;
                    }
                }

                // Add a separator between questions
                yPosition += 10;  // Adjust the gap between questions
                gfx.DrawString(new string('-', 40), font, XBrushes.Black, new XRect(20, yPosition, page.Width, 0));
                yPosition += 30; // Add space after the separator

                // Check if we need to create a new page (if the content overflows)
                if (yPosition > page.Height - 50)
                {
                    page = document.AddPage();
                    gfx = XGraphics.FromPdfPage(page);
                    yPosition = 20; // Reset yPosition for the new page
                }
            }

            // Save the document to the file path chosen by the user
            document.Save(filePath);
        }        
        public void ExportToTextFile(string filePath)
        {
            StringBuilder sb = new StringBuilder();

            if (IncludeNameField == true)
            {
                sb.AppendLine($"Name: _____________________________");
                sb.AppendLine("\n");
            }
            if (IncludeIDField == true)
            {
                sb.AppendLine($"ID: ________________");
                    sb.AppendLine("\n");
            }
            if (IncludeDateField == true)
            {
                sb.AppendLine($"Date: __/__/____");
                    sb.AppendLine("\n");
            }
            if (IncludeClassField == true)
            {
                sb.AppendLine($"Class: ___________");
                    sb.AppendLine("\n");
            }
            if (IncludeSectionField == true)
            {
                sb.AppendLine($"Section: ___________");
                    sb.AppendLine("\n");
            }
            if (IncludeGradeField == true)
            {
                if (NumberOfPoints > 0)
                {
                    sb.AppendLine($"Grade: ___/{NumberOfPoints}");
                        sb.AppendLine("\n");
                }
                else
                {
                    sb.AppendLine($"Grade: ___/___");
                        sb.AppendLine("\n");
                }
            }

            for (int j = 0; j < Questions.Count; j++)
            {
                Question question = Questions[j];
                sb.AppendLine($"Question {j+1}");
                sb.AppendLine($"({question.Points} points) {question.QuestionText}");
                if (question.Type == "Multiple Choice")
                {
                    for (int i = 0; i < question.Options.Count; i++)
                    {
                        sb.AppendLine($"{i + 1}. {question.Options[i]}");
                    }
                }
                else
                {
                    for (int i = 0; i < question.NumLines; i++)
                    {
                        sb.AppendLine("______________________________________________________________________________________");  // Placeholder line
                    }
                }
                sb.AppendLine(new string('-', 40));  // Separator between questions

            }
            File.WriteAllText(filePath, sb.ToString());
        }
        public void ExportToJsonFile(Exam exam, string filePath)
        {
            string jsonContent = JsonSerializer.Serialize(exam, new JsonSerializerOptions
            {
                WriteIndented = true // Makes the JSON more readable
            });

            // Save JSON to the specified file path
            File.WriteAllText(filePath, jsonContent);
        }
        // INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
