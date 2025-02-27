using Bank.Core.Models;
using Bank.DbAccess.Repositories;

namespace Bank.Cli;

public static class WithTransactions
{
    public static async void Run(IEnumerable<Ledger> ledgers, ILedgerRepository ledgerRepository)
    {
        ////////////////////
        // Your Code Here
        ////////////////////
        Console.WriteLine();
        Console.WriteLine("Booking, press ESC to stop.");

        var random = new Random();
        var allLedgersAsArray = ledgers.ToArray();
        while (!(Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Escape))
        {
            var from = allLedgersAsArray[random.Next(allLedgersAsArray.Length)];
            var to = allLedgersAsArray[random.Next(allLedgersAsArray.Length)];
            var amount = random.NextInt64(1, 101);
            //Console.Write(ledgerRepository.Book(amount, from, to)); !Uncomment this line
        }

        Console.WriteLine();
        ////////////////////
        // Your Code Here
        ////////////////////

        Console.WriteLine();
        Console.WriteLine("Getting total money in system at the end.");
        try
        {
            var endMoney = await ledgerRepository.GetTotalMoney();
            Console.WriteLine($"Total end money: {endMoney}");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error in getting total money.");
            Console.WriteLine(ex.Message);
        }
    }
}