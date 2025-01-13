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
        await using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        try
        {
            var totalMoney = await _context.Ledgers.OrderBy(ledger => ledger.Balance).CountAsync();
            await transaction.CommitAsync();
            return Convert.ToDecimal(totalMoney);
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
        await using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);
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

    public async Task<Ledger?> SelectOne(int id, MySqlConnection conn, MySqlTransaction? transaction)
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

    public async Task Update(Ledger ledger, MySqlConnection conn, MySqlTransaction? transaction)
    {
        try
        {
            _context.Ledgers.Add(new Ledger { Id = ledger.Id, Name = ledger.Name, Balance = ledger.Balance });
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public void Update(Ledger ledger)
    {
        using var conn = new MySqlConnection(_databaseSettings.ConnectionString);
        conn.Open();
        Update(ledger, conn, null);
    }

    public async Task<decimal?> GetBalance(int ledgerId, MySqlConnection conn, MySqlTransaction transaction)
    {
        try
        {
            var ledgers = await _context.Ledgers.OrderBy(ledger => ledgerId).ToListAsync();
            decimal balance = 0;
            foreach (var ledger in ledgers)
            {
                balance = ledger.Balance;
            }

            await transaction.CommitAsync();
            return balance;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}