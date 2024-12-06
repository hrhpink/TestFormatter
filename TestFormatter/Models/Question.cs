using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

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
        public BitmapImage QuestionImage { get; set; } // Property to store the image associated with the question
        public Tuple<List<string>,List<string>> Matching {  get; set; } // Only used for Mathcing
        public List<string> TrueOrFalse { get; set; }


        public void SetType(string newType)
        {
            Type = newType;
            if (newType == "Free Response")
            {
                Options = null; // Clear options for Free Response
            }
            else if (newType == "Multiple Choice")
            {
                if (Options == null)
                {
                    Options = new List<string>(); // Initialize options for Multiple Choice
                } 
            }
            else if (newType == "Matching")
            {
                Matching = new Tuple<List<string>, List<string>>(new List<string>(), new List<string>()); // Initialize matching for matching questions
            }
            else if (newType == "True/False")
            {
                TrueOrFalse = new List<string>(); // Initialize trueorfalse for T/F
            }
            else if (newType == "Fill in the Blank")
            {
                Options = null;
            }
        }
    }
}
