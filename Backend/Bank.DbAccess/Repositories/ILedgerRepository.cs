using Bank.Core.Models;
using MySqlConnector;

namespace Bank.DbAccess.Repositories;

public interface ILedgerRepository
{
    Task<IEnumerable<Ledger>> GetAllLedgers();
    public void Book(decimal amount, Ledger from, Ledger to);
    public void LoadBalance(Ledger ledger);
    decimal GetTotalMoney();
    Task<Ledger?> SelectOne(int id);
    Task<Ledger?> SelectOne(int id, MySqlConnection conn, MySqlTransaction? transaction);
    void Update(Ledger ledger, MySqlConnection conn, MySqlTransaction transaction);
    void Update(Ledger ledger);
    Task<decimal?> GetBalance(int ledgerId, MySqlConnection conn, MySqlTransaction transaction);
}