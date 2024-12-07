//Allows test to be imported from json file
namespace TestFormatter.Models
{
    public class ExamJsonWrapper
    {
        public List<Question> Questions { get; set; }
        public bool IncludeNameField { get; set; }
        public bool IncludeIDField { get; set; }
        public bool IncludeDateField { get; set; }
        public bool IncludeClassField { get; set; }
        public bool IncludeSectionField { get; set; }
        public bool IncludeGradeField { get; set; }
        public int QuestionLimit { get; set; } = 0;
        public double NumberOfPoints { get; set; }
        public int QuestionCount => Questions.Count;
    }
}