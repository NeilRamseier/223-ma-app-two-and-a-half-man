using Bank.DbAccess.Repositories;

namespace Bank.Cli;

public static class Simple
{
    public static void Run(ILedgerRepository ledgerRepository)
    {
        ////////////////////
        // Your Code Here
        ////////////////////

        Console.WriteLine("Booking, press ESC to stop.");

        var ledgerArray = ledgerRepository.GetAllLedgers().ToArray();

        var random = new Random();
        while (!Console.KeyAvailable || Console.ReadKey(true).Key != ConsoleKey.Escape)
        {
            var from = ledgerArray[random.Next(ledgerArray.Length)];
            var to = ledgerArray[random.Next(ledgerArray.Length)];

            while (to == from) to = ledgerArray[random.Next(ledgerArray.Length)];


            var amount = random.Next(1, 101);

           // ledgerRepository.Book(amount, from, to, conn, transaction);

            Console.Write(".");

            Thread.Sleep(100);
        }

        Console.WriteLine();
        Console.WriteLine("Getting total money in system at the end.");
        try
        {
            var startMoney = ledgerRepository.GetTotalMoney();
            Console.WriteLine($"Total end money: {startMoney}");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error in getting total money.");
            Console.WriteLine(ex.Message);
        }

        Console.WriteLine("Hello, World!");
    }
}