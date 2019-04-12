using iBank.Scanner.Data;

using System;
using System.Linq;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace iBank.Scanner
{
    public class MrzRecognizerPage : MrzPage
    {
        public event Func<object, MrzProcessedEventArgs, Task> OnMrzDetectedAsync;

        public async override Task RaiseMrzDetected(object sender, MrzRecognizedEventArgs e)
        {
            try
            {
                var lines = e.MrzCode.Split(new[] { Environment.NewLine, @"\\n", @"\n" }, StringSplitOptions.None);
                if (lines.Length != 2 || lines[0].Length != 42 || lines[1].Length != 44)
                    return;

                var fio = lines[0].Remove(0, 5).Split(new[] { "<" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                if (fio.Count < 2)
                    fio.Add(" ");

                var firstNameOld = fio[0].Replace(e.SurNames, "");
                var lastNameOld = e.SurNames;
                var patronymicOld = fio[1];

                var givenNames = e.GivenNames.Split(' ');
                var firstName = string.Join(" ", givenNames.Take(givenNames.Length - 1));
                var lastName = e.SurNames;
                var patronymic = givenNames.Last();

                var mrzParsed = new Mrz(lastName, firstName, patronymic, lines[1]);
                foreach (var @event in OnMrzDetectedAsync?.GetInvocationList())
                    await (@event as Func<object, MrzProcessedEventArgs, Task>)?.Invoke(sender, new MrzProcessedEventArgs(mrzParsed));
            }
            catch (Exception ex)
            {
                if (ex is FormatException)
                    return;
                await DisplayAlert("Внимание", ex.ToString(), "Окей");
            }
        }
    }
}