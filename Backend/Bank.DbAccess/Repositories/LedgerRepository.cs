using System.Data;
using Bank.Core.Models;
using Bank.DbAccess.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MySqlConnector;

namespace Bank.DbAccess.Repositories;

public class LedgerRepository : ILedgerRepository
{
    private readonly DatabaseSettings _databaseSettings;

    private readonly AppDbContext _context;

    public LedgerRepository(IOptions<DatabaseSettings> databaseSettings, AppDbContext context)
    {
        this._context = context;
        this._databaseSettings = databaseSettings.Value;
    }

    public void Book(decimal amount, Ledger from, Ledger to)
    {
        this.LoadBalance(from);
        from.Balance -= amount;
        this.Update(from);
        Thread.Sleep(250);
        this.LoadBalance(to);
        to.Balance += amount;
        this.Update(to);
    }

    public async Task<decimal> GetTotalMoney()
    {
        await using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);
        try
        {
            var totalMoney = await _context.Ledgers.SumAsync(ledger => ledger.Balance);
            await transaction.CommitAsync();
            return totalMoney
                ;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<IEnumerable<Ledger>> GetAllLedgers()
    {
        await using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        try
        {
            var allLedgers = await _context.Ledgers.OrderBy(ledger => ledger.Name).ToListAsync();
            await transaction.CommitAsync();
            return allLedgers;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task LoadBalance(Ledger ledger)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);
        try
        {
            var existingLedger = await _context.Ledgers.FirstOrDefaultAsync(l => l.Id == ledger.Id);
            ledger.Balance = existingLedger?.Balance ?? 0;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }


    public async Task<Ledger?> SelectOne(int id)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        try
        {
            var ledger = await _context.Ledgers.FirstOrDefaultAsync(l => l.Id == id);
            await transaction.CommitAsync();
            return ledger;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<Ledger?> SelectOne(int id, MySqlTransaction? transaction)
    {
        try
        {
            var ledger = await _context.Ledgers.FirstOrDefaultAsync(l => l.Id == id);
            await transaction.CommitAsync();
            return ledger;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task Update(Ledger ledger, MySqlTransaction transaction)
    {
        if (transaction == null)
        {
            throw new ArgumentNullException(nameof(transaction));
        }

        try
        {
            if (!_context.Ledgers.Any(l => l.Id == ledger.Id))
            {
                throw new Exception($"Ledger {ledger.Id} not found");
            }

            _context.Ledgers.Update(ledger);
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task Update(Ledger ledger)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);

        using var conn = new MySqlConnection(_databaseSettings.ConnectionString);
        conn.Open();
        await Update(ledger, null);
    }

    public async Task<decimal?> GetBalance(int ledgerId, MySqlTransaction transaction)
    {
        try
        {
            var ledger = await this.SelectOne(ledgerId, transaction);
            await transaction.CommitAsync();
            return ledger?.Balance;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task Delete(int id)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);

        try
        {
            var ledger = await _context.Ledgers.FirstOrDefaultAsync(u => u.Id == id);
            if (ledger == null)
            {
                throw new KeyNotFoundException($"No Ledger with id {id}");
            }

            _context.Ledgers.Remove(ledger);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}