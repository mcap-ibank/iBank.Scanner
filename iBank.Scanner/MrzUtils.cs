using System.Collections.Generic;
using System.Text;

namespace iBank.Scanner
{
    public static class MrzUtils
    {
        private static readonly List<char> Cyrillic = new List<char>()
        {
            ' ', 'А', 'Б', 'В', 'Г', 'Д', 'Е', 'Ё', 'Ж', 'З', 'И', 'Й', 'К', 'Л', 'М', 'Н', 'О', 'П', 'Р', 'С', 'Т', 'У', 'Ф', 'Х', 'Ц', 'Ч', 'Ш', 'Щ', 'Ъ', 'Ы', 'Ь', 'Э', 'Ю', 'Я'
        };
        private static readonly List<char> Latin = new List<char>()
        {
            ' ', 'A', 'B', 'V', 'G', 'D', 'E', '2', 'J', 'Z', 'I', 'Q', 'K', 'L', 'M', 'N', 'O', 'P', 'R', 'S', 'T', 'U', 'F', 'H', 'C', '3', '4', 'W', 'X', 'Y', '9', '6', '7', '8'
        };

        private static readonly StringBuilder StringBuilder = new StringBuilder();

        public static string ToCyrillic(string latin)
        {
            StringBuilder.Clear();
            foreach (var c in latin)
                StringBuilder.Append(Cyrillic[Latin.IndexOf(c)]);
            return StringBuilder.ToString();
        }
    }
}