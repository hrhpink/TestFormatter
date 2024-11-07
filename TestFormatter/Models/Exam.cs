using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.ComponentModel;


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
        public int NumberOfPoints { get; set; }

        public int QuestionCount => Questions.Count; // Property to track number of questions

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
            foreach (var question in Questions)
            {
                if (question.QuestionText == null)
                {
                    validationMessage = $"One or more of your questions are missing the question text.";
                    return false;
                }
                else if (question.Number <= 0 )
                {
                    validationMessage = $"Make sure all questions have a question number.";
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
                        validationMessage = $"Question {question.Number} is multiple choice but has no options.";
                        return false;
                    }
                }
                else if (question.Type == "Free Response")
                {
                    // Check if numLines is set for free response questions
                    if (question.NumLines <= 0)
                    {
                        validationMessage = $"Question {question.Number} is free response but has no placeholder lines specified.";
                        return false;
                    }
                }

            }

            validationMessage = ""; // No issues
            return true;
        }
        public void ExportToTextFile(string filePath)
            {
                StringBuilder sb = new StringBuilder();

                foreach (var question in Questions)
                {
                    sb.AppendLine($"Question {question.Number}");
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
                            sb.AppendLine("________");  // Placeholder line
                        }
                    }
                    sb.AppendLine(new string('-', 40));  // Separator between questions
                }
                File.WriteAllText(filePath, sb.ToString());
        }
        // INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
