namespace QuestBank.Core.QuestionAggregate
{
    public class Answer
    {
        public char Id { get; private set; }
        public string Text { get; private set; }

        public Answer(char id, string text)
        {
            Id = id;
            Text = text;
        }
    }
}