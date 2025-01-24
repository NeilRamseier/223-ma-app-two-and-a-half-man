using System;
using System.Transactions;
using Bank.Core.Models;
using Bank.DbAccess.Data;
using Bank.DbAccess.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MySqlConnector;
using TestProject1;
using Xunit.Abstractions;
using Xunit.Microsoft.DependencyInjection.Abstracts;
using static Bank.Core.Models.Ledger;
using static Bank.DbAccess.Repositories.IBookingRepository;

namespace BookingTest
{
    public class BookingRepositoryTest : IClassFixture<TestProjectFixture>
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly AppDbContext _dbContext;

        public BookingRepositoryTest(TestProjectFixture fixture)
        {
            var scope = fixture.ServiceProvider.CreateScope();
            _dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            _bookingRepository = scope.ServiceProvider.GetRequiredService<IBookingRepository>();
        }

        private void SeedDatabase()
        {
            _dbContext.Ledgers.RemoveRange(_dbContext.Ledgers);
            _dbContext.SaveChanges();
            _dbContext.Ledgers.Add(new Bank.Core.Models.Ledger { Id = 30, Balance = 200 });
            _dbContext.Ledgers.Add(new Bank.Core.Models.Ledger { Id = 31, Balance = 100 });
            _dbContext.SaveChanges();
        }

        [Fact]
        public async Task Book_TransferValidAmountBetweenLedgers()
        {
            decimal amount = 50;

            SeedDatabase();
            var sourceLedger = await _dbContext.Ledgers.FindAsync(30);
            var destinationLedger = await _dbContext.Ledgers.FindAsync(31);

            await _bookingRepository.Book(sourceLedger.Id, destinationLedger.Id, amount);

            Assert.Equal(150, sourceLedger.Balance);
            Assert.Equal(150, destinationLedger.Balance);
        }

        [Fact]
        public async Task Book_TransferMaxValidAmountBetweenLedgers()
        {
            decimal amount = 200;

            SeedDatabase();
            var sourceLedger = await _dbContext.Ledgers.FindAsync(30);
            var destinationLedger = await _dbContext.Ledgers.FindAsync(31);

            await _bookingRepository.Book(sourceLedger.Id, destinationLedger.Id, amount);

            Assert.Equal(0, sourceLedger.Balance);
            Assert.Equal(300, destinationLedger.Balance);
        }

        [Fact]
        public async Task Book_TransferInvalidAmountBetweenLedgers()
        {
            decimal amount = 300;

            SeedDatabase();
            var sourceLedger = await _dbContext.Ledgers.FindAsync(30);
            var destinationLedger = await _dbContext.Ledgers.FindAsync(31);

            var task = await _bookingRepository.Book(sourceLedger.Id, destinationLedger.Id, amount);
            Assert.False(task);
        }
    }
}