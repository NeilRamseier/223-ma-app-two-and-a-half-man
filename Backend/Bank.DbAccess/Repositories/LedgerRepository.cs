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


    public Ledger? SelectOne(int id)
    {
        Ledger? retLedger = null;
        bool worked;

        do
        {
            worked = true;
            using var conn = new MySqlConnection(_databaseSettings.ConnectionString);
            conn.Open();
            using var transaction = conn.BeginTransaction(IsolationLevel.Serializable);
            try
            {
                retLedger = SelectOne(id, conn, transaction);
            }
            catch (Exception ex)
            {
                // Attempt to roll back the transaction.
                try
                {
                    transaction.Rollback();
                    if (ex.GetType() != typeof(Exception))
                        worked = false;
                }
                catch (Exception ex2)
                {
                    // Handle any errors that may have occurred on the server that would cause the rollback to fail.
                    if (ex2.GetType() != typeof(Exception))
                        worked = false;
                }
            }
        } while (!worked);

        return retLedger;
    }

    public Ledger SelectOne(int id, MySqlConnection conn, MySqlTransaction? transaction)
    {
        const string query = $"SELECT id, name, balance FROM {Ledger.CollectionName} WHERE id=@Id";

        using var cmd = new MySqlCommand(query, conn, transaction);
        cmd.Parameters.AddWithValue("@Id", id);
        using var reader = cmd.ExecuteReader();
        if (!reader.Read())
            throw new Exception($"No Ledger with id {id}");

        var ordId = reader.GetInt32(reader.GetOrdinal("id"));
        var ordName = reader.GetString(reader.GetOrdinal("name"));
        var ordBalance = reader.GetDecimal(reader.GetOrdinal("balance"));

        return new Ledger { Id = ordId, Name = ordName, Balance = ordBalance };
    }

    public void Update(Ledger ledger, MySqlConnection conn, MySqlTransaction? transaction)
    {
        const string query = $"UPDATE {Ledger.CollectionName} SET name=@Name, balance=@Balance WHERE id=@Id";
        using var cmd = new MySqlCommand(query, conn, transaction);
        cmd.Parameters.AddWithValue("@Name", ledger.Name);
        cmd.Parameters.AddWithValue("@Balance", ledger.Balance);
        cmd.Parameters.AddWithValue("@Id", ledger.Id);

        cmd.ExecuteNonQuery();
    }

    public void Update(Ledger ledger)
    {
        using var conn = new MySqlConnection(_databaseSettings.ConnectionString);
        conn.Open();
        Update(ledger, conn, null);
    }

    public decimal? GetBalance(int ledgerId, MySqlConnection conn, MySqlTransaction transaction)
    {
        const string query = "SELECT balance FROM ledgers WHERE id=@Id";

        using var cmd = new MySqlCommand(query, conn, transaction);
        cmd.Parameters.AddWithValue("@Id", ledgerId);
        var result = cmd.ExecuteScalar();
        if (result != DBNull.Value)
        {
            return Convert.ToDecimal(result);
        }

        return null;
    }
}