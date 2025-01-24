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
        private readonly ITestOutputHelper output;

        public ConcurrentTests(TestProjectFixture fixture, ITestOutputHelper output)
        {
            var scope = fixture.ServiceProvider.CreateScope();
            _dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            _bookingRepository = scope.ServiceProvider.GetRequiredService<IBookingRepository>();
            this.output = output;
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
            const int numberOfBookings = 100;
            const int users = 10;
            Task[] tasks = new Task[users];


            var allLedgers = await _dbContext.Ledgers.OrderBy(ledger => ledger.Name).ToArrayAsync();
            decimal initialTotalBalance = allLedgers.Sum(l => l.Balance);
            var exceptions = new List<Exception>();
            var random = new Random();

            void UserAction()
            {
                for (int i = 0; i < numberOfBookings; i++)
                {
                    var from = allLedgers[random.Next(allLedgers.Length)];
                    var to = allLedgers[random.Next(allLedgers.Length)];

                    if (from.Id == to.Id) continue;
                    var amount = random.NextInt64(1, 101);
                    try
                    {
                        _bookingRepository.Book(from.Id, to.Id, amount);
                    }
                    catch (Exception ex)
                    {
                        lock (exceptions)
                        {
                            exceptions.Add(ex);
                        }
                    }
                }
            }

            for (int i = 0; i < users; i++)
            {
                tasks[i] = Task.Run(UserAction);
            }

            Task.WaitAll(tasks);

            Assert.Empty(exceptions);
            var finalTotalBalance = allLedgers.Sum(l => l.Balance);
            Assert.Equal(initialTotalBalance, finalTotalBalance);

            output.WriteLine($"Test completed successfully. Total balance: {finalTotalBalance}");
        }
    }
}