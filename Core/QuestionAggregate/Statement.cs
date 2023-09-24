namespace QuestBank.Core.QuestionAggregate
{
    public class Statement
    {
        public char Id { get; private set; }
        public string Text { get; private set; }
        
        public Statement(char id, string text)
        {
            Id = id;
            Text = text;
        }
    }
}