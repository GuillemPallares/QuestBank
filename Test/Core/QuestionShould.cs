using System.Configuration;
using System.Linq;
using QuestBank.Core.QuestionAggregate;
using QuestBank.Test.Helpers;

namespace QuestBank.Test.Core;

[TestClass]
public class QuestionShould
{
    [TestMethod]
    public void Question_Should_Load_From_Ctor()
    {
        var id = 0;

        var statements = new List<Statement>()
                                            {
                                                new Statement('A', "SatatementOne"),
                                                new Statement('B', "SatatementTwo"),
                                            };

        var answers = new List<Answer>()
                                            {
                                                new Answer('A', "AnswerOne"),
                                                new Answer('B', "AnswerTwo"),
                                                new Answer('C', "AnswerThree"),
                                                new Answer('D', "AnswerFour"),
                                                new Answer('E', "AnswerFive"),
                                                new Answer('F', "AnswerSix"),
                                            };

        var solutions = new Dictionary<char, string>()
        {
            {'A', "A"},
            {'B', "A"}
        };

        var sut = new Question(id, statements, answers, solutions);

        sut.Id.Should().Be(id);
        sut.Statements.Should().BeEquivalentTo(statements);
        sut.Answers.Should().BeEquivalentTo(answers);
        sut.Solutions.Should().BeEquivalentTo(solutions);
    }

    [TestMethod]
    [DataRow(1)]
    [DataRow(2)]
    [DataRow(3)]
    [DataRow(4)]
    [DataRow(5)]
    public void Question_Should_Add_Item(int itemsCount)
    {
        var sut = new Question(0);

        var statement = "TestStatement";
        var correctAnswer = "CorrectAnswer";
        var falseAnswer = "FalseAnswer";

        for (int i = 0; i < itemsCount; i++)
        {
            sut.AddItem(statement + i, correctAnswer + i, falseAnswer + i, falseAnswer + i);
        }

        sut.Solutions.Count().Should().Be(itemsCount);
        sut.Predefined.Count().Should().Be(itemsCount);
        sut.Predefined.Should().OnlyHaveUniqueItems();
        sut.Statements.Count().Should().Be(itemsCount);

        for (int i = 0; i < itemsCount; i++)
        {
            var stament = sut.Statements.ElementAt(i);
            stament.Id.Should().Be(Question.Ids[i]);
            stament.Text.Should().Be(statement + i);
            sut.Solutions.ElementAt(i).Key.Should().Be(Question.Ids[i]);
        }

        sut.Answers.Count().Should().Be(itemsCount * 3);

        for (int i = 0; i < itemsCount; i++)
        {
            var answerC = sut.Answers.ElementAt(i + (i * 2));
            answerC.Id.Should().Be(Question.Ids[i + (i * 2)]);
            answerC.Text.Should().Be(correctAnswer + i);
            sut.Solutions.ElementAt(i).Value.Contains(Question.Ids[i + (i * 2)]).Should().BeTrue();

            var answerF1 = sut.Answers.ElementAt(i + (i * 2) + 1);
            answerF1.Id.Should().Be(Question.Ids[i + (i * 2) + 1]);
            answerF1.Text.Should().Be(falseAnswer + i);

            var answerF2 = sut.Answers.ElementAt(i + (i * 2) + 2);
            answerF2.Id.Should().Be(Question.Ids[i + (i * 2) + 2]);
            answerF2.Text.Should().Be(falseAnswer + i);

        }
    }

    [TestMethod]
    [DataRow(0, 'A', "ABC")]
    [DataRow(0, 'B', "BFG")]
    [DataRow(0, 'A', "ABG")]
    [DataRow(0, 'B', "BFC")]
    [DataRow(0, 'A', "ABCD")]
    [DataRow(0, 'B', "BFGH")]
    [DataRow(0, 'A', "ABGH")]
    [DataRow(0, 'B', "BFCD")]
    [DataRow(0, 'A', "ABGHDF")]
    [DataRow(0, 'B', "BFCDEF")]
    public void Question_Should_Get_Item(int questionId, char statementId, string answersIds)
    {
        var sut = QuestionHelpers.CreateTestQuestion(0, 2, 8);

        var result = sut.GetItem($"{questionId}-{statementId}-{answersIds}");

        var statementResult = new KeyValuePair<char, string>('A', "AnswerOne");

        result.Statement.Key.Should().Be(statementId);
        result.Statement.Value.Should().Be(sut.Statements.First(s => s.Id == statementId).Text);
        result.Answers.Should().HaveCount(answersIds.Count());
        result.Answers.Should().ContainKeys(answersIds.ToArray());
        result.Answers.Should().ContainValues(sut.Answers.Where(a => answersIds.Contains(a.Id))
                                                                                                          .Select(x => x.Text)
                                                                   .ToArray());
    }

    [TestMethod]
    [DataRow(2)]
    [DataRow(3)]
    [DataRow(4)]
    [DataRow(5)]
    public void Question_Should_Generate_Item_With_N_Answers(int answerCount)
    {
        var sut = QuestionHelpers.CreateTestQuestion(0, 2, 8);

        var result = sut.GenerateItem(answerCount);

        result.Statement.Should().NotBeNull();
        result.Answers.Should().HaveCount(answerCount);
        result.Answers.Should().OnlyHaveUniqueItems();
    }

    [TestMethod]
    [DataRow('A')]
    [DataRow('B')]
    public void Question_Should_Generate_Item_With_Forced_Statement(char statementId)
    {
        var sut = QuestionHelpers.CreateTestQuestion(0, 2, 8);

        var result = new List<Item>();

        for (int i = 0; i < 100; i++)
        {
            result.Add(sut.GenerateItem(statementId: statementId));
        }

        result.All(r => r.Statement.Key == statementId).Should().BeTrue();
    }

    [TestMethod]
    [DataRow(new[] { 'A' })]
    [DataRow(new[] { 'A', 'B' })]
    [DataRow(new[] { 'A', 'B', 'C' })]
    public void Question_Should_Generate_Item_With_Forced_Answers(char[] answersIds)
    {
        var sut = QuestionHelpers.CreateTestQuestion(0, 2, 8);

        var result = new List<Item>();

        for (int i = 0; i < 100; i++)
        {
            result.Add(sut.GenerateItem(answersId: answersIds));
        }

        result.All(r => answersIds.All(a => r.Answers.Keys.Contains(a))).Should().BeTrue();
    }

    [TestMethod]
    [DataRow("0-A-ABCD", 'A')]
    [DataRow("0-B-ABCD", 'B')]

    public void Question_Should_Return_True_For_Correct_Answers(string itemId, char answer)
    {
        var sut = QuestionHelpers.CreateTestQuestion(0, 2, 8);

        var result = sut.IsCorrect(ItemId.Parse(itemId), answer);

        result.Should().BeTrue();
    }

    [TestMethod]
    [DataRow("0-A-ABCD", 'B')]
    [DataRow("0-B-ABCD", 'A')]

    public void Question_Should_Return_False_For_Incorrect_Answers(string itemId, char answer)
    {
        var sut = QuestionHelpers.CreateTestQuestion(0, 2, 8);

        var result = sut.IsCorrect(ItemId.Parse(itemId), answer);

        result.Should().BeFalse();
    }

    [Ignore]
    [TestMethod]
    [DataRow("0-A-ABCD", 'Z')]
    [DataRow("0-B-ABCD", 'Z')]

    public void Question_Should_Throw_Error_For_Non_Included_Answers(string itemId, char answer)
    {
        var sut = QuestionHelpers.CreateTestQuestion(0, 2, 8);

        var result = sut.IsCorrect(ItemId.Parse(itemId), answer);;
    }

    [Ignore]
    [TestMethod]
    [DataRow("1-A-ABCD", 'A')]
    [DataRow("1-B-ABCD", 'B')]

    public void Question_Should_Throw_Error_For_Non_Corresponding_Question(string itemId, char answer)
    {
        var sut = QuestionHelpers.CreateTestQuestion(0, 2, 8);

        var result = sut.IsCorrect(ItemId.Parse(itemId), answer);;
    }
}