using System.Data;
using System.Runtime.CompilerServices;
using Bank.DbAccess.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MySqlConnector;

namespace Bank.DbAccess.Repositories;

public class BookingRepository(IOptions<DatabaseSettings> settings, AppDbContext context) : IBookingRepository
{
    private DatabaseSettings _settings = settings.Value;

    private readonly AppDbContext _context = context;

    private LedgerRepository _ledgerRepository;


    public async Task<bool> Book(int sourceLedgerId, int destinationLKedgerId, decimal amount)
    {
        bool repeatTransaction = true;
        int repeatCounter = 0;
        bool result = false;

        await using var conn = new MySqlConnection(_settings.ConnectionString);
        await conn.OpenAsync();
        while (repeatTransaction)
        {

            await using var transaction =
                await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);

            var from = _context.Ledgers.FirstOrDefault(x => x.Id == sourceLedgerId);
            var to = _context.Ledgers.FirstOrDefault(x => x.Id == destinationLKedgerId);

            if (from == null || to == null)
            {
                result = false;
            }
            else
            {
                var possibleBalance = from.Balance -= amount;
                if (possibleBalance >= 0)
                {
                    from.Balance -= amount;
                    await _ledgerRepository.Update(from);
                    Thread.Sleep(250);
                    await _ledgerRepository.LoadBalance(to);
                    to.Balance += amount;
                    await _ledgerRepository.Update(to);
                    try
                    {
                        repeatTransaction = false;
                        result = true;
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
                }
            }
        }
        return result;
    }
}