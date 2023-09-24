using QuestBank.Core.QuestionAggregate;

namespace QuestBank.Test.Core
{
    [TestClass]
    public class ItemShould
    {
        [TestMethod]
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(3)]
        [DataRow(4)]
        [DataRow(5)]
        [DataRow(6)]
        public void ItemId_Should_Create_Empty_With_Number_Of_Questions(int answersCount)
        {
            var result = ItemId.Empty(0,answersCount);
        
            result.AnswersIds.Should().HaveCount(answersCount);
            result.ToString().Split("-")[2].Should().HaveLength(answersCount);
        }
    }
}