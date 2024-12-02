using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json;
using System.ComponentModel;
using System.Printing.IndexedProperties;

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
