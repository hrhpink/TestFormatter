using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestFormatter.Models
{
    public class Question
    {
        public string QuestionText { get; set; }
        public double Points { get; set; }
        public int Number { get; set; }
        public int NumLines { get; set; }
        public string Type { get; set; }
        public List<string> Options { get; set; } // Only used for Multiple Choice

        public void SetType(string newType)
        {
            Type = newType;
            if (newType == "Free Response")
            {
                Options = null; // Clear options for Free Response
            }
            else if (newType == "Multiple Choice")
            {
                Options = new List<string>(); // Initialize options for Multiple Choice
            }
        }
    }
}
