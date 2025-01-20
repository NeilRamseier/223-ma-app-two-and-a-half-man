using NBomber.Contracts.Stats;
using NBomber.CSharp;
using NBomber.Http.CSharp;

namespace LBank.Tests.Loadtest.Cli
{
    class Program
    {
        static async Task Main(string[] args)
        {
            /*  ApiService apiService = new ApiService();
              String jwt = await apiService.Login("admin", "adminpass");
              var ledgers = await apiService.GetLedgers(jwt);
                 foreach (Ledger ledger in ledgers)
                  {
                      Console.WriteLine(ledger.Name);
                  }

              Console.WriteLine("Press any key to exit");
              Console.ReadKey();*/

            using var httpClient = new HttpClient();

            var scenario = Scenario.Create("http_scenario", async context =>
                {
                    var request =
                        Http.CreateRequest("GET", "https://localhost:7183/api/v1/lbankinfo")
                            .WithHeader("Accept", "text/html");
                    // .WithHeader("Accept", "application/json")
                    // .WithBody(new StringContent("{ id: 1 }", Encoding.UTF8, "application/json");
                    // .WithBody(new ByteArrayContent(new [] {1,2,3}))


                    var response = await Http.Send(httpClient, request);

                    return response;
                })
                .WithoutWarmUp()
                .WithLoadSimulations(
                    Simulation.Inject(rate: 100,
                        interval: TimeSpan.FromSeconds(1),
                        during: TimeSpan.FromSeconds(30))
                );

            NBomberRunner
                .RegisterScenarios(scenario)
                .WithReportFileName("fetch_users_report")
                .WithReportFolder("fetch_users_reports")
                .WithReportFormats(ReportFormat.Html)
                .Run();

            Console.ReadKey();
        }
    }
}