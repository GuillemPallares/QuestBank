namespace QuestBank.Core.QuestionAggregate
{
    public class Item
    {
        public ItemId Id { get; init; }

        public int QuestionId { get; set; }

        public KeyValuePair<char,string> Statement { get; init; }

        public Dictionary<char, string> Answers { get; init; }
       
        public Item(int questionId, KeyValuePair<char, string> statement, Dictionary<char,string> answers)
        {
            Id = new ItemId(questionId, statement.Key, answers.Select(k => k.Key).ToArray());
            QuestionId = questionId;
            Statement = statement;
            Answers = answers;
        }
    }
}