using System.Net.Http.Headers;
using QuestBank.Core.QuestionAggregate;

namespace QuestBank.Test.Helpers
{
    public class QuestionHelpers
    {
        public static Question CreateTestQuestion(int id = 0, int statements = 1, int answers = 4)
        {
            
            var _solutions = new Dictionary<char,string>();
            var _statements = new List<Statement>();
            for (var i = 0; i < statements; i++)
            {
                _statements.Add(new Statement(Question.Ids[i], $"Statement{Question.Ids[i]}"));
                _solutions.Add(Question.Ids[i], Question.Ids[i].ToString());
            };

            var _answers = new List<Answer>();
            for (var i = 0; i < answers; i++)
            {
                _answers.Add(new Answer(Question.Ids[i], $"Answer{Question.Ids[i]}"));
            };

            return new Question(id, _statements, _answers, _solutions);
        }
    }
}