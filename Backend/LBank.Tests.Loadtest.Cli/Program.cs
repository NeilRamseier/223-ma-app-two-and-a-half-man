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

            using var httpClient = new HttpClient();


            var bookingData = new
            {
                customerId = "12345",
                bookingDate = DateTime.UtcNow.AddDays(1).ToString("yyyy-MM-ddTHH:mm:ss"),
                amount = 100.00
            };
            string jsonContent = JsonSerializer.Serialize(bookingData);


            var scenario = Scenario.Create("booking_api_scenario", async context =>
                {
                    var request =
                        Http.CreateRequest("POST", "https://localhost:7183/api/v1/bookings")
                            .WithHeader("Accept", "application/json")
                            .WithHeader("Authorization", $"Bearer {jwt}")
                            .WithBody(new StringContent(jsonContent, Encoding.UTF8, "application/json"));

                    var response = await Http.Send(httpClient, request);

                    return response;
                })
                .WithoutWarmUp()
                .WithLoadSimulations(
                    Simulation.Inject(rate: 50,
                        interval: TimeSpan.FromSeconds(1),
                        during: TimeSpan.FromSeconds(30))
                );


            NBomberRunner
                .RegisterScenarios(scenario)
                .WithReportFileName("booking_api_report")
                .WithReportFolder("booking_api_reports")
                .WithReportFormats(ReportFormat.Html, ReportFormat.Txt)
                .Run();

            Console.WriteLine("Stresstest abgeschlossen. Report wurde erstellt.");
            Console.ReadKey();
        }
    }
}