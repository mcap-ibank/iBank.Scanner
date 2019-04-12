using System.Threading.Tasks;

namespace iBank.Scanner
{
    public class MrzViewerPage : MrzRecognizerPage
    {
        public MrzViewerPage()
		{
            Title = "Просмотр MRZ";
            OnMrzDetectedAsync += MrzViewerPage_OnMrzDetectedAsync;
        }

        private Task MrzViewerPage_OnMrzDetectedAsync(object sender, MrzProcessedEventArgs e) => DisplayAlert("Успех",
$@"ФИО:
{e.Result.LastName}
{e.Result.FirstName}
{e.Result.Patronymic}

Дата рождения:
{e.Result.BirthDate.ToShortDateString()}

Паспорт:
{e.Result.PassportSerial}

Дата выдачи паспорта:
{e.Result.PassportIssueDate.ToShortDateString()}

Код подразделения:
{e.Result.PassportDivisionCode}", "Окей");
    }
}