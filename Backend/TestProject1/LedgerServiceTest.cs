using Bank.Core.Models;
using Bank.DbAccess;
using Bank.DbAccess.Data;
using Bank.DbAccess.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Options;
using Moq;
namespace TestProject1;

public class LedgerRepositoryTests
{
    private readonly DbContextOptions<AppDbContext> _options;
    private readonly Mock<IOptions<DatabaseSettings>> _mockOptions;

    public LedgerRepositoryTests()
    {
        _options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            // don't raise the error warning us that the in memory db doesn't support transactions
            .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        var databaseSettings = new DatabaseSettings
        {
            ConnectionString = "FakeConnectionString"  // Mocked connection string
        };

        _mockOptions = new Mock<IOptions<DatabaseSettings>>();
        _mockOptions.Setup(o => o.Value).Returns(databaseSettings);
    }

    [Fact]
    public async Task Delete_ValidId_RemovesLedgerFromDatabase()
    {
        await using var context = new AppDbContext(_options);
        var repository = new LedgerRepository(_mockOptions.Object, context);
        var ledger = new Ledger { Id = 1, Name = "Test Ledger", Balance = 100 };
        context.Ledgers.Add(ledger);
        await context.SaveChangesAsync();

        await repository.Delete(1);

        var result = await context.Ledgers.FindAsync(1);
        Assert.Null(result);  // The ledger should be removed
    }

    [Fact]
    public async Task Delete_InvalidId_ThrowsKeyNotFoundException()
    {
        using var context = new AppDbContext(_options); 
        var repository = new LedgerRepository(_mockOptions.Object, context);
        await Console.Out.WriteLineAsync("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA");
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() => repository.Delete(999));
        await Console.Out.WriteLineAsync("LJSDKJFLSAJDFÖJSADFÖKJSAÖDLJFÖLAKJSDÖLFJSÖLAJFÖLDSKJAF");
        await Console.Out.WriteLineAsync(exception.Message);
        Assert.Equal("No Ledger with id 999", exception.Message);
    }
    [Fact]
    public async Task Delete_TransactionIsRolledBackOnFailure()
    {
        using var context = new AppDbContext(_options);
        var repository = new LedgerRepository(_mockOptions.Object, context);
        var ledger = new Ledger { Id = 2, Name = "Test Ledger", Balance = 100 };
        context.Ledgers.Add(ledger);
        await context.SaveChangesAsync();

        await Assert.ThrowsAsync<KeyNotFoundException>(() => repository.Delete(99));  // Nonexistent ID

        var existingLedger = await context.Ledgers.FindAsync(2);
        Assert.NotNull(existingLedger);  // Ledger still exists
    }



}
