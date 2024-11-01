using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestFormatter.Models
{
    public abstract class Question
    {
        public string QuestionText { get; set; }
        public int Points { get; set; }
        public abstract string Type { get; }
        public int Number {  get; set; }
        public int NumLines {  get; set; }
    }

    public class FreeResponseQuestion : Question
    {
        public override string Type => "Free Response";
    }

    public class MultipleChoiceQuestion : Question
    {
        public override string Type => "Multiple Choice";
        public List<string> Options { get; set; } = new List<string>();
    }
}
