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

            Console.WriteLine($"Anfangsbetrag: {initialTotalAmount}");

            using var httpClient = new HttpClient();

            // 3. Szenario für Lasttests definieren
            var scenario = Scenario.Create("http_scenario", async context =>
                {
                    var request =
                        Http.CreateRequest("GET", "https://localhost:7183/api/v1/lbankinfo")
                            .WithHeader("Authorization", $"Bearer {jwt}")
                            .WithHeader("Accept", "application/json");

                    var response = await Http.Send(httpClient, request);

                    return response;
                })
                .WithoutWarmUp()
                .WithLoadSimulations(
                    Simulation.Inject(rate: 100,
                        interval: TimeSpan.FromSeconds(1),
                        during: TimeSpan.FromSeconds(30))
                );

            // 4. Szenario ausführen
            NBomberRunner
                .RegisterScenarios(scenario)
                .WithReportFileName("fetch_users_report")
                .WithReportFolder("fetch_users_reports")
                .WithReportFormats(ReportFormat.Html)
                .Run();

            // 5. Ledgers nach dem Test abrufen
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

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }
    }
}