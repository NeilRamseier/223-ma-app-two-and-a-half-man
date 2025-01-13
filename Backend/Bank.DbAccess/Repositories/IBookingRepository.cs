namespace Bank.DbAccess.Repositories;

public interface IBookingRepository
{
    Task<bool> Book(int sourceLedgerId, int destinationLKedgerId, decimal amount);
}