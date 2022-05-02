namespace VkParser.Core.Toolkit;

public class TextMacros
{
    private static readonly Random _rnd = new();

    public static string CreateString(int lengthFrom, int lengthTo, string pattern, string customSymbols = "") =>
        RandomString(_rnd.Next(lengthFrom < lengthTo ? lengthFrom : lengthTo, lengthTo + 1), pattern, customSymbols);

    public static string RandomString(int length, string pattern, string customSymbols = "")
    {
        var dic = new Dictionary<char, string>
        {
            { 'a', "ABCDEFGHIJKLMNOPQRSTUVWXYZ" },
            { 'b', "abcdefghijklmnopqrstuvwxyz" },
            { 'c', "0123456789" },
            { 'd', "$^%#*" }
        };

        var temp = new List<string>();
        var chars = new char[length];

        Array.ForEach(pattern.ToCharArray(), p => temp.Add(dic[char.ToLower(p)]));
        if (!string.IsNullOrEmpty(customSymbols)) temp.Add(customSymbols);

        var symbols = temp.OrderBy(item => _rnd.Next()).ToArray();
        var line = symbols.Aggregate((acc, val) => acc + val).ToString();

        for (var i = 0; i < chars.Length; i++)
            chars[i] = line[_rnd.Next(line.Length)];

        if (chars.Length >= symbols.Length)
        {
            for (var i = 0; i < symbols.Length; i++)
            {
                if (!string.IsNullOrEmpty(symbols[i]))
                    chars[i] = Convert.ToChar(symbols[i].Substring(_rnd.Next(symbols[i].Length), 1));
            }
        }

        return string.Join("", chars);
    }
}