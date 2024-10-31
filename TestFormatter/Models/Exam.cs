using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestFormatter.Models
{
    public class Exam
    {
        public List<Question> Questions { get; private set; } = new List<Question>();

        public void AddQuestion(Question question)
        {
            Questions.Add(question);
        }
    }
}
