using System.Collections;
using System.Net.Mail;

namespace QuestBank.Core.QuestionAggregate
{
    public class Question
    {

        public int Id { get; private set; }

        public List<Statement> Statements { get; private set; }

        public List<Answer> Answers { get; private set; }

        public Dictionary<char, string> Solutions { get; private set;}

        public List<ItemId> Predefined { get; private set;}

        public const string Ids = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";


        private Question()
        {
            Statements = new();
            Answers = new();
            Solutions = new();
            Predefined = new();
        }
        public Question(int id)
            : this()
        {
            Id = id;
        }
        public Question(int id, List<Statement> statements , List<Answer> answers, Dictionary<char, string> solutions)
            : this()
        {
            Id = id;
            Statements = statements;
            Answers = answers;
            Solutions = solutions;
        }

        public char AddStatement(string statement)
        {
            var statementId = Statements.Count == 0 
                                    ? Ids.First() 
                                    : Ids[Ids.IndexOf(Statements.Last().Id) + 1];
            Statements.Add(new Statement(statementId, statement));
            
            return statementId;
        }

        public char AddAnswer(string answer)
        {
            var answerId = Answers.Count == 0 
                                    ? Ids.First() 
                                    : Ids[Ids.IndexOf(Answers.Last().Id) + 1];

            Answers.Add(new Answer(answerId, answer));

            return answerId;
        }

        public void AddSolution(char statementId, char answerId)
        {
            if(!Solutions.TryAdd(statementId, answerId.ToString()))
                Solutions[statementId] += answerId;
        }

        public ItemId AddItem(string statement, string correctAnswer, params string[] answers)
        {
            var statementId = AddStatement(statement);
            var answersIds = new char[]{AddAnswer(correctAnswer)};

            AddSolution(statementId, answersIds.First());

            foreach (var answer in answers)
            {
               answersIds = answersIds.Append(AddAnswer(answer)).ToArray();
            }

            var itemId = new ItemId(Id, statementId, answersIds);

            Predefined.Add(itemId);

            return itemId;
        }

        /// <summary>
        /// Gets the Item corresponding to the given ItemId
        /// </summary>
        /// <param name="itemId">The Id of the Item to be returned</param>
        /// <returns>Item</returns> <summary>
        public Item GetItem(string id)
        {
            var itemId = ItemId.Parse(id);

            return GenerateItem(itemId.AnswersIds.Length, itemId.StatementId, itemId.AnswersIds.ToArray());
        }   

        /// <summary>
        /// Generate a new item.
        /// </summary>
        /// <param name="answersCount">The number of answers the Item should have</param>
        /// <param name="statementId">The Id of the stament you want to force</param>
        /// <param name="answersId">The Id of the answers you want to force</param>
        /// <returns></returns>
        public Item GenerateItem(int answersCount = 4, char? statementId = null, params char[] answersId)
        {
            var statement = statementId is not null 
                                ? Statements.First(s => s.Id == statementId)
                                : Statements.ElementAt(Random.Shared.Next(0, Statements.Count));

            var answers = Answers.Where(a => answersId.Contains(a.Id)).ToList();

            if(answers.All(a => !Solutions[statement.Id].Contains(a.Id)))
            {
                if(answers.Count == answersCount) //Pendiente de crear excepciones de dominio.
                    throw new ArgumentOutOfRangeException("No Correct answers can be added");

                answers.Add(Answers.First(a => a.Id == Solutions[statement.Id].First()));
            }
    

            if(answers.Count < answersCount)
            {
                var loops = answersCount - answers.Count;
                for (int i = 0; i < loops; i++)
                {
                    var availableAnswers = Answers.Except(answers).ToArray();

                    answers.Add(availableAnswers.ElementAt(Random.Shared.Next(0, availableAnswers.Length)));
                }
            }

            return new Item(Id, 
                             new KeyValuePair<char, string>(statement.Id, statement.Text),
                             answers.ToDictionary(k => k.Id, v => v.Text));
        }

        public bool IsCorrect(ItemId id, char answer)
        {
            if(id.QuestionId != Id)
                throw new NotImplementedException();

            if(!id.AnswersIds.Contains(answer))
                throw new NotImplementedException();

            return Solutions[id.StatementId].Contains(answer);
        }
    }
}