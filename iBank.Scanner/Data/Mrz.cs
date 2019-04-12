using System;
using System.Globalization;

namespace iBank.Scanner.Data
{
    public class Mrz
    {
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Patronymic { get; set; }
        public DateTime BirthDate { get; set; }
        public string PassportSerial { get; set; }
        public DateTime PassportIssueDate { get; set; }
        public string PassportDivisionCode { get; set; }

        public Mrz() { }
        public Mrz(string lastName, string firstName, string patronymic, string mrzLine2)
        {
            FirstName = MrzUtils.ToCyrillic(firstName);
            LastName = MrzUtils.ToCyrillic(lastName);
            Patronymic = MrzUtils.ToCyrillic(patronymic);

            var bd = mrzLine2.Substring(13, 6).Replace("O", "0").Replace("o", "0");
            BirthDate = DateTime.ParseExact(bd, "yyMMdd", CultureInfo.InvariantCulture);
            var psn1 = mrzLine2.Substring(0, 3);
            var psn2 = mrzLine2.Substring(28, 1);
            var psn3 = mrzLine2.Substring(3, 6);
            var psn4 = $"{psn1}{psn2}{psn3}";
            PassportSerial = psn4.Insert(2, " ").Insert(5, " ").Replace("O", "0").Replace("o", "0");
            var pd = mrzLine2.Substring(29, 6).Replace("O", "0").Replace("o", "0");
            PassportIssueDate = DateTime.ParseExact(pd, "yyMMdd", CultureInfo.InvariantCulture);
            PassportDivisionCode = mrzLine2.Substring(mrzLine2.Length - 3 - 6, 6).Insert(3, "-").Replace("O", "0").Replace("o", "0");
        }

        public void Normalize()
        {
            LastName = CultureInfo.InvariantCulture.TextInfo.ToTitleCase(LastName);
            FirstName = CultureInfo.InvariantCulture.TextInfo.ToTitleCase(LastName);
            Patronymic = CultureInfo.InvariantCulture.TextInfo.ToTitleCase(LastName);
        }
    }
}