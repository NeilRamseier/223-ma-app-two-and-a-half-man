using System;
using Bank.Core.Models;
using Bank.DbAccess.Repositories;
using MySqlConnector;
using Xunit.Abstractions;
using static Bank.Core.Models.Ledger;
using static Bank.DbAccess.Repositories.IBookingRepository;

namespace BookingTest
{
    public class BookingRepositoryTest
    {
        private readonly ITestOutputHelper output;

        public BookingRepositoryTest(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void Book_TransferValidAmountBetweenLedgers()
        {
            var sourceLedgerId = 1;
            var desinationLedgerId = 2;
            decimal amount = 100;
            
            var sourceLedger = new Ledger { Id = sourceLedgerId, Balance = 200};
            var desinationLedger = new Ledger { Id = desinationLedgerId, Balance = 50};
            
            bookingRepository.Book(sourceLedger.Id, desinationLedger.Id, amount);
            
            Assert.Equal(100, sourceLedger.Balance);
            Assert.Equal(150, desinationLedger.Balance);
        }
    }
}