using System.Text;

namespace QuestBank.Core.Helpers
{
    public static class StringHelpers
    {
        public static string Repeat(this char parameter, int times)
        {
            var builder = new StringBuilder();
            for (var i = 0; i < times; i++)
            {
                builder.Append(parameter);
            }

            return builder.ToString();
        }
    }
}