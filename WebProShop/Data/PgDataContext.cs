using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebProShop.Data.Models;

namespace WebProShop.Data
{
    public class PgDataContext : DbContext
    {
        private readonly string connectionString;

        public PgDataContext(string connectionString)
        {
            this.connectionString = connectionString;
        }

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

        private class UnitOfWork : IUnitOfWork
        {
            private readonly PgDataContext context;
            private readonly Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction transaction;
            private bool isCommitted;

            public UnitOfWork(PgDataContext context)
            {
                this.context = context;
                this.transaction = context.Database.BeginTransaction();
            }

            public void Add<T>(T item)
            {
                context.Add(item);
            }

            public Task CommitAsync()
            {
                isCommitted = true;
                return this.transaction.CommitAsync();
            }

            public async ValueTask DisposeAsync()
            {
                if(isCommitted)
                {
                    return;
                }

                await this.transaction.RollbackAsync();
            }

            public void Remove<T>(T item)
            {
                this.context.Remove(item);
            }

            public Task RollbackAsync()
            {
                isCommitted = true;
                return transaction.RollbackAsync();
            }

            public Task SaveChangesAsync()
            {
                return context.SaveChangesAsync();
            }

            public void Update<T>(T item)
            {
                context.Update(item);
            }
        }
    }
}
