
using Bank.DbAccess.Data;
using Bank.DbAccess.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;
using Xunit.Microsoft.DependencyInjection.Abstracts;


namespace Bank.Concurrent.Test


{
    public class ConcurrentTests : IClassFixture<TestProjectFixture>
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly AppDbContext _dbContext;

        public ConcurrentTests(TestProjectFixture fixture)
        {
            var scope = fixture.ServiceProvider.CreateScope();
            _dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            _bookingRepository = scope.ServiceProvider.GetRequiredService<IBookingRepository>();
            SeedDatabase();
        }

        private void SeedDatabase()
        {
            // Seed data for tests
            const int totalLedgers = 100;
            _dbContext.Ledgers.RemoveRange(_dbContext.Ledgers);
            _dbContext.SaveChanges();

            for (int i = 0; i < totalLedgers; i++)
            {
                _dbContext.Ledgers.Add(new Bank.Core.Models.Ledger { Id = i + 1, Balance = 1000m });
            }
 
            _dbContext.SaveChanges();
        }

        [Fact]
        public async Task TestBookingParallel()
        {
            const int numberOfBookings = 1;
            const int users = 2;
            Task[] tasks = new Task[users];

            
            var allLedgers = await _dbContext.Ledgers.OrderBy(ledger => ledger.Name).ToArrayAsync();

            async Task UserAction()
            {
                Random random = new Random();
                for (int i = 0; i < numberOfBookings; i++)
                {
                    // Book methode testen
                    // Implementieren Sie hier die parallelen Buchungen
                    // Bestimmen sie zwei zufällige Ledgers
                    var from = allLedgers[random.Next(allLedgers.Length-1)];
                    var to = allLedgers[random.Next(allLedgers.Length-1)];
                    var amount = random.NextInt64(1, 101);
                    try
                    {
                        _bookingRepository.Book(from.Id, to.Id, amount);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
            }

            for (int i = 0; i < users; i++)
            {
                tasks[i] = Task.Run(UserAction);
            }

            Task.WaitAll(tasks);
        }
    }
}