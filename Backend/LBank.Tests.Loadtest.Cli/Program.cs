using System.Text;
using System.Text.Json;
using LBank.Tests.Loadtest.Cli.Services;
using NBomber.Contracts.Stats;
using NBomber.CSharp;
using NBomber.Http.CSharp;

namespace LBank.Tests.Loadtest.Cli
{
    class Program
    {
        static async Task Main(string[] args)
        {
            ApiService apiService = new ApiService();

            string jwt = await apiService.Login("admin", "adminpass");

            var initialLedgers = await apiService.GetLedgers(jwt);
            decimal initialTotalAmount = initialLedgers.Sum(ledger => ledger.Balance);

            var scenario = Scenario.Create("booking_api_scenario", async context =>
                {
                    using var httpClient = new HttpClient();

                    using var timeout = new CancellationTokenSource();
                    timeout.CancelAfter(TimeSpan.FromSeconds(3));


                    Random random = new Random();

                    int randomSource = random.Next(1, 5);
                    int randomDestination = random.Next(1, 5);


                    var bookingData = new
                    {
                        sourceId = randomSource,
                        destinationId = randomDestination,
                        amount = 1,
                    };

                    string jsonContent = JsonSerializer.Serialize(bookingData);

                    var request = Http.CreateRequest("POST", "http://localhost:5000/api/v1/bookings")
                        .WithHeader("Accept", "application/json")
                        .WithHeader("Authorization", $"Bearer {jwt}")
                        .WithBody(new StringContent(jsonContent, Encoding.UTF8, "application/json"));

                    //var clientArgs = HttpClientArgs.Create(timeout.Token);

                    var response = await Http.Send(httpClient, request);


                    return response;
                })
                .WithoutWarmUp()
                .WithLoadSimulations(
                    Simulation.Inject(
                        rate: 50,
                        interval: TimeSpan.FromSeconds(1),
                        during: TimeSpan.FromSeconds(30))
                );

            NBomberRunner
                .RegisterScenarios(scenario)
                .WithReportFileName("booking_api_report")
                .WithReportFolder("booking_api_reports")
                .WithReportFormats(ReportFormat.Html, ReportFormat.Txt, ReportFormat.Csv)
                .Run();


            Console.WriteLine("Lasttest abgeschlossen. Report wurde erstellt.");
            Console.ReadKey();


            var finalLedgers = await apiService.GetLedgers(jwt);
            decimal finalTotalAmount = finalLedgers.Sum(ledger => ledger.Balance);
            decimal difrence = finalTotalAmount - initialTotalAmount;
            Console.WriteLine($"Anfangsbetrag: {initialTotalAmount}");
            Console.WriteLine($"Endbetrag: {finalTotalAmount}");
            Console.WriteLine($"Differenz: {difrence}");
            // 6. Vergleich der Beträge
            if (initialTotalAmount == finalTotalAmount)
            {
                Console.WriteLine("Der Geldbetrag hat sich nicht geändert.");
            }
            else
            {
                Console.WriteLine(
                    $"Der Geldbetrag hat sich geändert. Differenz: {finalTotalAmount - initialTotalAmount}");
            }
        }
    }
}