using LBank.Tests.Loadtest.Cli.Services;

namespace LBank.Tests.Loadtest.Cli
{
    class Program
    {
        static async Task Main(string[] args)
        {
            ApiService apiService = new ApiService();
            String jwt = await apiService.Login("admin", "adminpass");
            var ledgers = await apiService.GetLedgers(jwt);
            /*    foreach (Ledger ledger in ledgers)
                {
                    Console.WriteLine(ledger.Name);
                }*/

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }
    }
}