using Bank.Core.Models;
using MySqlConnector;

namespace Bank.DbAccess.Repositories;

public interface ILedgerRepository
{
    Task<IEnumerable<Ledger>> GetAllLedgers();
    public void Book(decimal amount, Ledger from, Ledger to);
    public Task LoadBalance(Ledger ledger);
    Task<decimal> GetTotalMoney();
    Task<Ledger?> SelectOne(int id);
    Task<Ledger?> SelectOne(int id, MySqlTransaction? transaction);
    Task Update(Ledger ledger, MySqlTransaction transaction);
    Task Update(Ledger ledger);
    Task Delete(int id);
    Task<decimal?> GetBalance(int ledgerId, MySqlTransaction transaction);
    Task<Ledger?> Create(Ledger ledger);
}