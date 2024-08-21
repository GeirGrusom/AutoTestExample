using System;
using System.Threading.Tasks;

namespace WebProShop.Data;

public interface IUnitOfWork : IAsyncDisposable
{
    void Add<T>(T item);
    void Remove<T>(T item);
    void Update<T>(T item);

    Task SaveChangesAsync();

    Task CommitAsync();
    Task RollbackAsync();
}
