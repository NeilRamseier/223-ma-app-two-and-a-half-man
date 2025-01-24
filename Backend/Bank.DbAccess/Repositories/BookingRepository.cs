using System.Data;
using Bank.DbAccess.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Bank.DbAccess.Repositories;

public class BookingRepository(
    IOptions<DatabaseSettings> settings,
    AppDbContext context,
    ILedgerRepository ledgerRepository) : IBookingRepository
{
    private DatabaseSettings _settings = settings.Value;

    public async Task<bool> Book(int sourceLedgerId, int destinationLKedgerId, decimal amount)
    {
        var repeatCounter = 0;

        while (repeatCounter++ < 1000)
        {
            await using var transaction =
                await context.Database.BeginTransactionAsync(IsolationLevel.Serializable);

            var from = context.Ledgers.FirstOrDefault(x => x.Id == sourceLedgerId);
            var to = context.Ledgers.FirstOrDefault(x => x.Id == destinationLKedgerId);

            if (from == null || to == null)
            {
                return false;
            }

            var possibleBalance = from.Balance - amount;
            if (possibleBalance < 0)
            {

                return false;
            }

            from.Balance -= amount;
            context.Ledgers.Update(from);
            Thread.Sleep(250);
            to.Balance += amount;
            context.Ledgers.Update(to);
            try
            {
                await context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();

            }
        }

        return false;
    }
}