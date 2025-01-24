using System.Data;
using System.Runtime.CompilerServices;
using Bank.DbAccess.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MySqlConnector;

namespace Bank.DbAccess.Repositories;

public class BookingRepository(IOptions<DatabaseSettings> settings, AppDbContext context, ILedgerRepository ledgerRepository) : IBookingRepository
{
    private DatabaseSettings _settings = settings.Value;

    public async Task<bool> Book(int sourceLedgerId, int destinationLKedgerId, decimal amount)
    {
        bool repeatTransaction = true;
        int repeatCounter = 0;
        bool result = false;

        while (repeatTransaction)
        {

            await using var transaction =
                await context.Database.BeginTransactionAsync(IsolationLevel.Serializable);

            var from = context.Ledgers.FirstOrDefault(x => x.Id == sourceLedgerId);
            var to = context.Ledgers.FirstOrDefault(x => x.Id == destinationLKedgerId);

            if (from == null || to == null)
            {
                result = false;
            }
            else
            {
                var fromBalance = from.Balance;
                var possibleBalance = fromBalance - amount;
                if (possibleBalance >= 0)
                {
                    from.Balance -= amount;
                    context.Ledgers.Update(from);
                    Thread.Sleep(250);
                    to.Balance += amount;
                    context.Ledgers.Update(to);
                    try
                    {
                        repeatTransaction = false;
                        result = true;
                        await context.SaveChangesAsync();
                        await transaction.CommitAsync();
                    }
                    catch (Exception e)
                    {
                        await transaction.RollbackAsync();
                        repeatCounter++;
                        throw;
                    }
                }

                {
                    result = false;
                    repeatTransaction = false;
                }
            }
        }
        return result;
    }
}