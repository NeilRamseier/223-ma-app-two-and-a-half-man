using Bank.Core.Models;
using Bank.DbAccess.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bank.Web.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class LedgersController(ILedgerRepository ledgerRepository) : ControllerBase
{
    [HttpGet]
    [Authorize(Roles = "Administrators,Users")]
    public Task<IEnumerable<Ledger>> Get()
    {
        var allLedgers = ledgerRepository.GetAllLedgers();
        return allLedgers;
    }

    [HttpGet("{id:int}")]
    [Authorize(Roles = "Administrators,Users")]
    public Task<Ledger?> Get(int id)
    {
        var ledger = ledgerRepository.SelectOne(id);
        return ledger;
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Administrators")]
    public void Put(int id, [FromBody] Ledger ledger)
    {
        ledgerRepository.Update(ledger);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Administrators")]
    public async Task Delete(int id)
    {
        await ledgerRepository.Delete(id);
    }

    [HttpPost]
    [Authorize(Roles = "Administrators")]
    public async Task Create([FromBody] Ledger ledger)
    {
        await ledgerRepository.Create(ledger);
    }
}