using iBank.Core.Utils;
using iBank.Scanner.Data;

using OfficeOpenXml;
using OfficeOpenXml.Style;

using Plugin.FilePicker;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace iBank.Scanner
{
    public interface ISaveShit
    {
        byte[] Data { get; set; }
        void SaveFile(byte[] data);
    }

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class VerifyInputPage : ContentPage
    {
        public static ISaveShit SaveShit { get; set; }

        public static byte[] ExcelShit { get; set; }

        private List<ExcelPersonExtended> LoadedPersons { get; set; } = new List<ExcelPersonExtended>();
        private MrzRecognizerPage MrzViewerPage { get; }

        public VerifyInputPage()
        {
            InitializeComponent();
            MrzViewerPage = new MrzRecognizerPage();
            MrzViewerPage.OnMrzDetectedAsync += MrzViewerPage_OnMrzDetected;
        }

        private async Task MrzViewerPage_OnMrzDetected(object sender, MrzProcessedEventArgs e)
        {
            e.Result.Normalize();

            var result = LoadedPersons.SingleOrDefault(p => p.PassportSerial.Equals(e.Result.PassportSerial));
            if (result != null)
            {
                bool err = false;
                if (!result.LastName.Equals(e.Result.LastName))
                {
                    err = true;
                    var res = await DisplayActionSheet(
$@"Имя
{result.LastName} | {e.Result.LastName}
Заменить?", "Нет", "Да");
                    if (res.Equals("Да", StringComparison.OrdinalIgnoreCase))
                        result.LastName = e.Result.LastName;
                }
                if (!result.FirstName.Equals(e.Result.FirstName))
                {
                    err = true;
                    var res = await DisplayActionSheet(
$@"Фамилия
{result.FirstName} | {e.Result.FirstName}
Заменить?", "Нет", "Да");
                    if (res.Equals("Да", StringComparison.OrdinalIgnoreCase))
                        result.FirstName = e.Result.FirstName;
                }
                if (!result.Patronymic.Equals(e.Result.Patronymic))
                {
                    err = true;
                    var res = await DisplayActionSheet(
$@"Отчество (Excel|Копия)
{result.Patronymic} | {e.Result.Patronymic}
Заменить?", "Нет", "Да");
                    if (res.Equals("Да", StringComparison.OrdinalIgnoreCase))
                        result.Patronymic = e.Result.Patronymic;
                }
                if (!result.BirthDate.Equals(e.Result.BirthDate))
                {
                    err = true;
                    var res = await DisplayActionSheet(
$@"Дата рождения (Excel|Копия)
{result.BirthDate.ToShortDateString()} | {e.Result.BirthDate.ToShortDateString()}
Заменить?", "Нет", "Да");
                    if (res.Equals("Да", StringComparison.OrdinalIgnoreCase))
                        result.BirthDate = e.Result.BirthDate;
                }
                if (!result.PassportIssueDate.Equals(e.Result.PassportIssueDate))
                {
                    err = true;
                    var res = await DisplayActionSheet(
$@"Дата паспорта (Excel|Копия)
{result.PassportIssueDate.ToShortDateString()} | {e.Result.PassportIssueDate.ToShortDateString()}
Заменить?", "Нет", "Да");
                    if (res.Equals("Да", StringComparison.OrdinalIgnoreCase))
                        result.PassportIssueDate = e.Result.PassportIssueDate;
                }
                if (!result.PassportDivisionCode.Equals(e.Result.PassportDivisionCode))
                {
                    err = true;
                    var res = await DisplayActionSheet(
$@"Код подразделения (Excel|Копия)
{result.PassportDivisionCode} | {e.Result.PassportDivisionCode}
Заменить?", "Нет", "Да");
                    if (res.Equals("Да", StringComparison.OrdinalIgnoreCase))
                        result.PassportDivisionCode = e.Result.PassportDivisionCode;
                }
                if (!err)
                {
                    await DisplayAlert("Успех",
  $@"Данные верны

Место рождения:
{result.BirthPlace}

Паспорт выдан:
{result.PassportIssue}

Д. телефон:
{result.PhoneHome}

М. телефон:
{result.PhoneMobile}

Кодовое слово:
{result.Codeword}

Военкомат:
{result.RecruitmentOfficeID}", "Окей");
                    result.IsCorrect = true;
                }
            }
            else
            {
                await DisplayAlert("Ошибка!", "Призывник не найден в электронке", "Окей");
                LoadedPersons.Add(new ExcelPersonExtended(new Core.Excel.ExcelPerson()
                {
                    PassportSerial = e.Result.PassportSerial,
                    LastName = e.Result.LastName,
                    FirstName = e.Result.FirstName,
                    Patronymic = e.Result.Patronymic,
                    BirthDate = e.Result.BirthDate,
                    PassportIssueDate = e.Result.PassportIssueDate,
                    PassportDivisionCode = e.Result.PassportDivisionCode,
                    Codeword = e.Result.LastName
                }) { IsCorrect = false });
            }
        }

        private async void Button_PickFile_Clicked(object sender, EventArgs e)
        {
            var excelFile = await CrossFilePicker.Current.PickFile();
            Label1.Text = excelFile.FileName;
            LoadedPersons = ExcelUtils.LoadInputForm(excelFile.FilePath, excelFile.DataArray).Select(p => new ExcelPersonExtended(p)).ToList();
            Label2.Text = $"Кол-во: {LoadedPersons.Count}";
        }

        private async void Button_Verify_Clicked(object sender, EventArgs e) => await Navigation.PushAsync(MrzViewerPage);

        private void Button_RebuildFile_Clicked(object sender, EventArgs e)
        {
            byte[] data;
            using (var p = new ExcelPackage(new MemoryStream(ExcelShit)))
            {
                var workbook = p.Workbook;
                var sheet = workbook.Worksheets.First();

                var correctPersons = LoadedPersons.Where(lp => lp.IsCorrect).ToList();
                var incorrectPersons = LoadedPersons.Where(lp => !lp.IsCorrect).ToList();

                int row = 4;
                // Проверенные призывники
                for (var i = 0; i < correctPersons.Count; i++, row++)
                {
                    if (LoadedPersons[i].IsCorrect)
                    {
                        sheet.Row(row).Style.Fill.PatternType = ExcelFillStyle.Solid;
                        sheet.Row(row).Style.Fill.BackgroundColor.SetColor(Color.Yellow);
                    }
                    sheet.Cells[$"A{row}"].Value = row - 3; //id
                    sheet.Cells[$"B{row}"].Value = correctPersons[i].LastName; // f
                    sheet.Cells[$"C{row}"].Value = correctPersons[i].FirstName; // i
                    sheet.Cells[$"D{row}"].Value = correctPersons[i].Patronymic; // o
                    sheet.Cells[$"E{row}"].Value = correctPersons[i].BirthDate.ToShortDateString(); // bd
                    sheet.Cells[$"F{row}"].Value = correctPersons[i].BirthPlace; // bp
                    sheet.Cells[$"G{row}"].Value = correctPersons[i].PassportSerial; // psn
                    sheet.Cells[$"H{row}"].Value = correctPersons[i].PassportIssue; // pi
                    sheet.Cells[$"I{row}"].Value = correctPersons[i].PassportIssueDate.ToShortDateString(); // pid
                    sheet.Cells[$"J{row}"].Value = correctPersons[i].PassportDivisionCode; // pdc
                    sheet.Cells[$"K{row}"].Value = correctPersons[i].Address; // addr
                    sheet.Cells[$"L{row}"].Value = "Москва"; //
                    sheet.Cells[$"M{row}"].Value = correctPersons[i].PhoneHome;
                    sheet.Cells[$"N{row}"].Value = correctPersons[i].PhoneMobile;
                    sheet.Cells[$"O{row}"].Value = correctPersons[i].Codeword;
                    sheet.Cells[$"P{row}"].Value = correctPersons[i].RecruitmentOfficeID;
                }
                row++; // Пауза в строку
                // Ошибочные призывники
                for (var i = 0; i < incorrectPersons.Count; i++, row++)
                {
                    if (!LoadedPersons[i].IsCorrect)
                    {
                        sheet.Row(row).Style.Fill.PatternType = ExcelFillStyle.Solid;
                        sheet.Row(row).Style.Fill.BackgroundColor.SetColor(Color.White);
                    }
                    sheet.Cells[$"A{row}"].Value = row - 4; //id
                    sheet.Cells[$"B{row}"].Value = incorrectPersons[i].LastName; // f
                    sheet.Cells[$"C{row}"].Value = incorrectPersons[i].FirstName; // i
                    sheet.Cells[$"D{row}"].Value = incorrectPersons[i].Patronymic; // o
                    sheet.Cells[$"E{row}"].Value = incorrectPersons[i].BirthDate.ToShortDateString(); // bd
                    sheet.Cells[$"F{row}"].Value = incorrectPersons[i].BirthPlace; // bp
                    sheet.Cells[$"G{row}"].Value = incorrectPersons[i].PassportSerial; // psn
                    sheet.Cells[$"H{row}"].Value = incorrectPersons[i].PassportIssue; // pi
                    sheet.Cells[$"I{row}"].Value = incorrectPersons[i].PassportIssueDate.ToShortDateString(); // pid
                    sheet.Cells[$"J{row}"].Value = incorrectPersons[i].PassportDivisionCode; // pdc
                    sheet.Cells[$"K{row}"].Value = incorrectPersons[i].Address; // addr
                    sheet.Cells[$"L{row}"].Value = "Москва"; //
                    sheet.Cells[$"M{row}"].Value = incorrectPersons[i].PhoneHome;
                    sheet.Cells[$"N{row}"].Value = incorrectPersons[i].PhoneMobile;
                    sheet.Cells[$"O{row}"].Value = incorrectPersons[i].Codeword;
                    sheet.Cells[$"P{row}"].Value = incorrectPersons[i].RecruitmentOfficeID;
                }

                data = p.GetAsByteArray();
            }
            SaveShit.SaveFile(data);
        }
    }
}