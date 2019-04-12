using iBank.Core;

using System;
using System.Threading.Tasks;

namespace iBank.Scanner
{
    public class MrzDatabaseVerifierPage : MrzRecognizerPage
    {
        public MrzDatabaseVerifierPage()
        {
            Title = "Сверка MRZ с БД";

            OnMrzDetectedAsync += MrzVerifierPage_OnMrzDetectedAsync;
        }

        private async Task MrzVerifierPage_OnMrzDetectedAsync(object sender, MrzProcessedEventArgs e)
        {
            var sql = $@"
SELECT last_name, first_name, patronymic, birth_date, passport_issue_date, passport_division_code, birth_place, passport_issue
FROM person 
WHERE passport_serial = '{e.Result.PassportSerial}'";
            using (var sqlcmd = new SqlCommandExecutor(sql))
            using (var sqlReader = sqlcmd.ExecuteReader())
            {
                if (sqlReader.Read())
                {
                    var lastNameSQL = sqlReader.GetString(0).Replace('-', ' ');
                    var fisrtNameSQL = sqlReader.GetString(1).Replace('-', ' ');
                    var patronymicSQL = sqlReader.GetString(2).Replace('-', ' ');
                    var birthDateSQL = sqlReader.GetDateTime(3);
                    var passportDateSQL = sqlReader.GetDateTime(4);
                    var passportDCSQL = sqlReader.GetString(5);
                    var birthPlace = sqlReader.GetString(6);
                    var passportIssue = sqlReader.GetString(7);

                    bool err = false;
                    if (!e.Result.LastName.Equals(lastNameSQL, StringComparison.OrdinalIgnoreCase))
                    {
                        await DisplayAlert("Ошибка",
$@"Имя (ДБ|Копия)
{lastNameSQL} | {e.Result.LastName}", "Окей");
                        err = true;
                    }
                    if (!e.Result.FirstName.Equals(fisrtNameSQL, StringComparison.OrdinalIgnoreCase))
                    {
                        await DisplayAlert("Ошибка",
$@"Фамилия (ДБ|Копия)
{fisrtNameSQL} | {e.Result.FirstName}", "Окей");
                        err = true;
                    }
                    if (!e.Result.Patronymic.Equals(patronymicSQL, StringComparison.OrdinalIgnoreCase))
                    {
                        await DisplayAlert("Ошибка",
$@"Отчество (ДБ|Копия)
{patronymicSQL}| {e.Result.Patronymic}", "Окей");
                        err = true;
                    }
                    if (!e.Result.BirthDate.Equals(birthDateSQL))
                    {
                        await DisplayAlert("Ошибка",
$@"Дата рождения (ДБ|Копия)
{birthDateSQL} | {e.Result.BirthDate}", "Окей");
                        err = true;
                    }
                    if (!e.Result.PassportIssueDate.Equals(passportDateSQL))
                    {
                        await DisplayAlert("Ошибка",
$@"Дата паспорта (ДБ|Копия)
{passportDateSQL} | {e.Result.PassportIssueDate}", "Окей");
                        err = true;
                    }
                    if (!e.Result.PassportDivisionCode.Equals(passportDCSQL))
                    {
                        await DisplayAlert("Ошибка",
$@"Код подразделения (ДБ|Копия)
{passportDCSQL} | {e.Result.PassportDivisionCode}", "Окей");
                        err = true;
                    }

                    if (!err)
                    {
                        await DisplayAlert("Успех",
$@"Данные верны

Место рождения:
{birthPlace}

Паспорт выдан:\n{passportIssue}", "Окей");
                    }
                }
                else
                    await DisplayAlert("Внимание", "Призывник не найден", "Окей");
            }
        }
    }
}