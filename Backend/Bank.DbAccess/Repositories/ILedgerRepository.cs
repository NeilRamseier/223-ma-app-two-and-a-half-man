using Bank.Core.Models;
using Microsoft.EntityFrameworkCore.Storage;
using MySqlConnector;

namespace Bank.DbAccess.Repositories;

public interface ILedgerRepository
{
    Task<IEnumerable<Ledger>> GetAllLedgers();
    public void Book(decimal amount, Ledger from, Ledger to);
    public Task LoadBalance(Ledger ledger);
    Task<decimal> GetTotalMoney();
    Task<Ledger?> SelectOne(int id);
    Task<Ledger?> SelectOne(int id, IDbContextTransaction? transaction);
    Task Update(Ledger ledger, IDbContextTransaction transaction);
    Task Update(Ledger ledger);
    Task Delete(int id);
    Task<decimal?> GetBalance(int ledgerId, IDbContextTransaction transaction);
    Task<Ledger?> Create(Ledger ledger);
}