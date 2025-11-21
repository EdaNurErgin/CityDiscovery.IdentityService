using IdentityService.Application.Interfaces;
using IdentityService.Infrastructure.Data;

namespace IdentityService.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IdentityDbContext _db;
        public UnitOfWork(IdentityDbContext db) => _db = db;
        public Task<int> SaveChangesAsync() => _db.SaveChangesAsync();

        // BURAYI düzelt:
        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
            => _db.SaveChangesAsync(cancellationToken);
    }
}
