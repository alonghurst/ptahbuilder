using System.Text.RegularExpressions;

namespace PtahBuilder.Tests.TestBuilder.Entities.Dice;

public static class DiceParser
{
    //private static Regex _regex = new Regex(@"(\\(([^()]|(?R))*\\))");

    private static Regex _regex = new("\\(((?>[^()]+|\\((?<n>)|\\)(?<-n>))+(?(n)(?!)))\\)");

    public static bool TryParse(string text, out IDiceValue value)
    {
        try
        {
            value = Parse(text);
            return true;
        }
        catch
        {
            value = new ConstantDiceValue(0);
            return false;
        }
    }

    public static IDiceValue Parse(string text)
    {
        text = text.Replace(" ", string.Empty).Trim();

        var opens = text.Count(x => x == '(');
        var closes = text.Count(x => x == ')');

        if (opens == 1 && text.StartsWith("(") && text.EndsWith(")"))
        {
            text = text.Substring(1, text.Length - 2);
        }

        if (closes != opens)
        {
            throw new InvalidOperationException($"Mismatched open and close brackets ({opens} vs {closes})");
        }

        var isAdd = text.Contains("+");
        var isMinus = text.Contains("-");

        if (isAdd || isMinus)
        {
            // It's an equation, split

            // (((2 + 5) + 3) + 4) + (2 + 9)
            var parts = _regex.Matches(text);

            if (parts.Count > 0)
            {
                while (parts.First().Value.Length == text.Length)
                {
                    text = text.Substring(1, text.Length - 2);
                    parts = _regex.Matches(text);
                }

                var l = parts.First().Value;
                var r = text.Replace(l, string.Empty);

                // Remove the operator from the right side
                r = r.Trim().Substring(1, r.Length - 1);

                var a = Parse(l);
                var b = Parse(r);

                return new DiceValue(a, b, isMinus ? EquationOperator.Subtract : EquationOperator.Add);
            }
            else
            {
                text = text.Replace("(", string.Empty).Replace(")", string.Empty);

                var splits = text.Split(new[] { "+", "-" }, StringSplitOptions.RemoveEmptyEntries);

                var l = Parse(splits[0]);
                var r = Parse(splits[1]);

                return new DiceValue(l, r, isMinus ? EquationOperator.Subtract : EquationOperator.Add);
            }
        }

        if (int.TryParse(text, out var value))
        {
            return new ConstantDiceValue(value);
        }

        return Dice.Parse(text);
    }
}