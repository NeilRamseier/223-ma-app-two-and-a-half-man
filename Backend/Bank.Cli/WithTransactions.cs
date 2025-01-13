using System.Data;
using Bank.Core.Models;
using Bank.DbAccess;
using Bank.DbAccess.Repositories;
using Microsoft.Extensions.Options;
using MySqlConnector;

namespace Bank.Cli;

public static class WithTransactions
{
    public static void Run(IEnumerable<Ledger> ledgers, ILedgerRepository ledgerRepository, DatabaseSettings databaseSettings)
    {
        Console.WriteLine();
        Console.WriteLine("Booking, press ESC to stop.");

        using (var conn = new MySqlConnection(databaseSettings.ConnectionString))
        {
            conn.Open();
            using (var transaction = conn.BeginTransaction(IsolationLevel.Serializable))
            {
                try
                {
                    var random = new Random();
                    var allLedgersAsArray = ledgers.ToArray();
                    while (!(Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Escape))
                    {
                        var from = allLedgersAsArray[random.Next(allLedgersAsArray.Length)];
                        var to = allLedgersAsArray[random.Next(allLedgersAsArray.Length)];
                        var amount = random.NextInt64(1, 101);
                        Console.Write(ledgerRepository.Book(amount, from, to, conn, transaction));
                    }

                    transaction.Commit(); // Commit the transaction if all bookings succeed
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred during booking.");
                    Console.WriteLine(ex.Message);
                    try
                    {
                        transaction.Rollback(); // Roll back in case of error
                    }
                    catch (Exception rollbackEx)
                    {
                        Console.WriteLine("Rollback failed.");
                        Console.WriteLine(rollbackEx.Message);
                    }
                }
            }
        }

        Console.WriteLine();
        Console.WriteLine("Getting total money in system at the end.");
        try
        {
            decimal endMoney = ledgerRepository.GetTotalMoney();
            Console.WriteLine($"Total end money: {endMoney}");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error in getting total money.");
            Console.WriteLine(ex.Message);
        }
    }
}