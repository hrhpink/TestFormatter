using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestFormatter.Models
{
    public abstract class Question
    {
        public string Text { get; set; }
        public int Points { get; set; }
        public abstract string Type { get; }
        public int Number {  get; set; }
    }

    public class ShortAnswerQuestion : Question
    {
        public override string Type => "Short Answer";
    }

    public class LongAnswerQuestion : Question
    {
        public override string Type => "Long Answer";
    }

    public class MultipleChoiceQuestion : Question
    {
        public override string Type => "Multiple Choice";
        public List<string> Options { get; set; } = new List<string>();
    }
}
