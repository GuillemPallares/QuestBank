using System.Text;

namespace QuestBank.Core.QuestionAggregate
{
    public struct ItemId
    {
        public int QuestionId { get; private set; }

        public char StatementId { get; private set; }

        public char[] AnswersIds { get; private set; }

        public ItemId(int questionId, char statementId, params char[] answersIds)
        {
            QuestionId = questionId;
            StatementId = statementId;
            AnswersIds = answersIds;
        }

        public static ItemId Empty(int QuestionId, int answers = 4)
            => new ItemId(QuestionId, '#', '#'.Repeat(answers).ToArray());

        public static ItemId Parse(string id)
        {
            var splitId = id.Split("-");
            return new ItemId(questionId: int.Parse(splitId[0]), char.Parse(splitId[1]), splitId[2].ToArray());
        }


        public override string ToString()
            => $"{QuestionId.ToString()}-{StatementId}-{string.Concat(AnswersIds)}";
    }
}