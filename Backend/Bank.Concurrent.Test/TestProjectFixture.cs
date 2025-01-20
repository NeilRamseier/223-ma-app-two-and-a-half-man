using Bank.DbAccess;
using Bank.DbAccess.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Microsoft.DependencyInjection;
using Xunit.Microsoft.DependencyInjection.Abstracts;

namespace Bank.Concurrent.Test;

public class TestProjectFixture : TestBedFixture
{
    protected override void AddServices(IServiceCollection services, IConfiguration? configuration)
        => services
            .AddTransient<IDatabaseSeeder, DatabaseSeeder>()
            .AddTransient<ILedgerRepository, LedgerRepository>()
            .AddTransient<IUserRepository, UserRepository>()
            .AddTransient<IBookingRepository, BookingRepository>();

    protected override ValueTask DisposeAsyncCore()
        => new();

    protected override IEnumerable<TestAppSettings> GetTestAppSettings()
    {
        yield return new() { Filename = "appsettings.json", IsOptional = false };
    }
    
}