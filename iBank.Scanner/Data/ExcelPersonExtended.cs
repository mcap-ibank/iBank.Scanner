using iBank.Core.Excel;

namespace iBank.Scanner.Data
{
    public class ExcelPersonExtended : ExcelPerson
    {
        public bool IsCorrect { get; set; }

        public ExcelPersonExtended(ExcelPerson person)
        {
            PassportSerial = person.PassportSerial;
            LastName = person.LastName;
            FirstName = person.FirstName;
            Patronymic = person.Patronymic;
            BirthDate = person.BirthDate;
            BirthPlace = person.BirthPlace;
            PassportIssue = person.PassportIssue;
            PassportIssueDate = person.PassportIssueDate;
            PassportDivisionCode = person.PassportDivisionCode;
            Address = person.Address;
            PhoneHome = person.PhoneHome;
            PhoneMobile = person.PhoneMobile;
            RecruitmentOfficeID = person.RecruitmentOfficeID;
            Codeword = person.Codeword;
            Comment = person.Comment;
        }
    }
}