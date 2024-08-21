using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using WebProShop.Data.Models;

namespace WebProShop.Data;

public class PgDataContext(string connectionString) : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            .UseNpgsql(connectionString)
            .UseSnakeCaseNamingConvention();
    }

    public IUnitOfWork CreateUnitOfWork() => new UnitOfWork(this);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .Entity<Product>(m =>
            {
                m.HasKey(x => x.Id);
                m.Property(x => x.Name);
                m.Property(x => x.Description);
                m.Property(x => x.Price);
            })
            .Entity<ShoppingCart>(m =>
            {
                m.HasKey(x => x.Id);
                m.HasMany(x => x.Lines).WithOne();
            })
            .Entity<ShoppingCartLine>(m =>
            {
                m.HasKey("ShoppingCartId", "Id");
                m.Property(x => x.Amount);
            })
        ;
    }

    [ExcludeFromCodeCoverage]
    private sealed class UnitOfWork(PgDataContext context) : IUnitOfWork
    {
        private readonly Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction transaction = context.Database.BeginTransaction();
        private bool isCommitted;

        public void Add<T>(T item)
        {
            context.Add(item);
        }

        public Task CommitAsync()
        {
            isCommitted = true;
            return transaction.CommitAsync();
        }

        public async ValueTask DisposeAsync()
        {
            if(isCommitted)
            {
                return;
            }

            await transaction.RollbackAsync();
        }

        public void Delete<T>(T item)
        {
            context.Remove(item);
        }

        public Task RollbackAsync()
        {
            isCommitted = true;
            return transaction.RollbackAsync();
        }

        public Task SaveChangesAsync() => context.SaveChangesAsync();

        public void Update<T>(T item) => context.Update(item);
    }
}
